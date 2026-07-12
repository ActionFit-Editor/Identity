using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ActionFit.Identity.Tests
{
    public class InstallationIdentityServiceTests
    {
        [Test]
        public void Resolve_PreservesStoredIdBeforeMigrationSources()
        {
            var store = new MemoryStore("stored-id");
            var source = new MemorySource("legacy", "legacy-id");
            var service = CreateService(store, new[] { source }, "generated-id");

            InstallationIdResolution result = service.Resolve();

            Assert.That(result.InstallationId, Is.EqualTo("stored-id"));
            Assert.That(result.Kind, Is.EqualTo(InstallationIdResolutionKind.Stored));
            Assert.That(source.LoadCount, Is.Zero);
            Assert.That(store.SaveCount, Is.Zero);
        }

        [Test]
        public void Resolve_MigratesFirstValidCandidateInOrder()
        {
            var store = new MemoryStore();
            var emptySource = new MemorySource("empty", "");
            var firstSource = new MemorySource("first", "legacy-id");
            var laterSource = new MemorySource("later", "later-id");
            var service = CreateService(store, new[] { emptySource, firstSource, laterSource }, "generated-id");

            InstallationIdResolution result = service.Resolve();

            Assert.That(result.InstallationId, Is.EqualTo("legacy-id"));
            Assert.That(result.Kind, Is.EqualTo(InstallationIdResolutionKind.Migrated));
            Assert.That(result.MigrationSource, Is.EqualTo("first"));
            Assert.That(store.Value, Is.EqualTo("legacy-id"));
            Assert.That(store.SaveCount, Is.EqualTo(1));
            Assert.That(laterSource.LoadCount, Is.Zero);
        }

        [Test]
        public void Resolve_GeneratesAndPersistsIdWhenNoCandidateExists()
        {
            var store = new MemoryStore();
            var service = CreateService(store, new[] { new MemorySource("empty", " ") }, "generated-id");

            InstallationIdResolution result = service.Resolve();

            Assert.That(result.InstallationId, Is.EqualTo("generated-id"));
            Assert.That(result.Kind, Is.EqualTo(InstallationIdResolutionKind.Generated));
            Assert.That(store.Value, Is.EqualTo("generated-id"));
            Assert.That(store.SaveCount, Is.EqualTo(1));
        }

        [Test]
        public void Resolve_ReturnsPersistedGeneratedIdOnLaterCalls()
        {
            var store = new MemoryStore();
            var generator = new MemoryGenerator("generated-id");
            var service = new InstallationIdentityService(store, generator: generator);

            string first = service.GetOrCreateId();
            InstallationIdResolution second = service.Resolve();

            Assert.That(first, Is.EqualTo("generated-id"));
            Assert.That(second.InstallationId, Is.EqualTo(first));
            Assert.That(second.Kind, Is.EqualTo(InstallationIdResolutionKind.Stored));
            Assert.That(generator.CreateCount, Is.EqualTo(1));
        }

        [Test]
        public void ReplaceId_UpdatesDurableValue()
        {
            var store = new MemoryStore("old-id");
            var service = CreateService(store, null, "generated-id");

            service.ReplaceId("recovered-id");

            Assert.That(service.GetOrCreateId(), Is.EqualTo("recovered-id"));
            Assert.That(store.SaveCount, Is.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ReplaceId_RejectsEmptyValues(string invalidId)
        {
            var store = new MemoryStore("stored-id");
            var service = CreateService(store, null, "generated-id");

            Assert.Throws<ArgumentException>(() => service.ReplaceId(invalidId));
            Assert.That(store.Value, Is.EqualTo("stored-id"));
            Assert.That(store.SaveCount, Is.Zero);
        }

        [Test]
        public void Resolve_RejectsInvalidGeneratorResultWithoutSaving()
        {
            var store = new MemoryStore();
            var service = CreateService(store, null, "");

            Assert.Throws<InvalidOperationException>(() => service.GetOrCreateId());
            Assert.That(store.SaveCount, Is.Zero);
        }

        [Test]
        public void Resolve_PropagatesCanonicalReadFailureWithoutGenerating()
        {
            var generator = new MemoryGenerator("generated-id");
            var store = new DelegateInstallationIdStore(
                () => throw new InvalidOperationException("read failed"),
                _ => Assert.Fail("A failed canonical read must not save a replacement ID."));
            var service = new InstallationIdentityService(store, generator: generator);

            Assert.Throws<InvalidOperationException>(() => service.GetOrCreateId());
            Assert.That(generator.CreateCount, Is.Zero);
        }

        [Test]
        public void Resolve_PropagatesMigrationReadFailureWithoutGenerating()
        {
            var store = new MemoryStore();
            var generator = new MemoryGenerator("generated-id");
            var source = new DelegateInstallationIdMigrationSource(
                "failing-source",
                () => throw new InvalidOperationException("migration read failed"));
            var service = new InstallationIdentityService(store, new[] { source }, generator);

            Assert.Throws<InvalidOperationException>(() => service.GetOrCreateId());
            Assert.That(generator.CreateCount, Is.Zero);
            Assert.That(store.SaveCount, Is.Zero);
        }

        [Test]
        public void GuidGenerator_ReturnsGuidCompatibleValue()
        {
            string generatedId = new GuidInstallationIdGenerator().CreateId();

            Assert.That(Guid.TryParse(generatedId, out _), Is.True);
        }

        private static InstallationIdentityService CreateService(
            MemoryStore store,
            IEnumerable<IInstallationIdMigrationSource> sources,
            string generatedId)
        {
            return new InstallationIdentityService(store, sources, new MemoryGenerator(generatedId));
        }

        private sealed class MemoryStore : IInstallationIdStore
        {
            public MemoryStore(string value = "") => Value = value;

            public string Value { get; private set; }
            public int SaveCount { get; private set; }

            public string LoadId() => Value;

            public void SaveId(string installationId)
            {
                Value = installationId;
                SaveCount++;
            }
        }

        private sealed class MemorySource : IInstallationIdMigrationSource
        {
            private readonly string _value;

            public MemorySource(string name, string value)
            {
                Name = name;
                _value = value;
            }

            public string Name { get; }
            public int LoadCount { get; private set; }

            public string LoadCandidate()
            {
                LoadCount++;
                return _value;
            }
        }

        private sealed class MemoryGenerator : IInstallationIdGenerator
        {
            private readonly string _value;

            public MemoryGenerator(string value) => _value = value;

            public int CreateCount { get; private set; }

            public string CreateId()
            {
                CreateCount++;
                return _value;
            }
        }
    }
}
