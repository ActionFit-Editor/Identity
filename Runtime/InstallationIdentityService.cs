using System;
using System.Collections.Generic;

namespace ActionFit.Identity
{
    public sealed class InstallationIdentityService
    {
        private readonly object _gate = new object();
        private readonly IInstallationIdStore _store;
        private readonly List<IInstallationIdMigrationSource> _migrationSources;
        private readonly IInstallationIdGenerator _generator;

        public InstallationIdentityService(
            IInstallationIdStore store,
            IEnumerable<IInstallationIdMigrationSource> migrationSources = null,
            IInstallationIdGenerator generator = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _generator = generator ?? new GuidInstallationIdGenerator();
            _migrationSources = new List<IInstallationIdMigrationSource>();

            if (migrationSources == null) return;

            foreach (var source in migrationSources)
            {
                if (source != null) _migrationSources.Add(source);
            }
        }

        /// <summary>
        /// Resolves the durable installation ID without changing a valid stored value.
        /// When no stored value exists, the first valid migration candidate is persisted before a new ID is generated.
        /// </summary>
        public InstallationIdResolution Resolve()
        {
            lock (_gate)
            {
                string storedId = _store.LoadId();
                if (HasValue(storedId))
                {
                    return new InstallationIdResolution(storedId, InstallationIdResolutionKind.Stored);
                }

                foreach (var source in _migrationSources)
                {
                    string candidate = source.LoadCandidate();
                    if (!HasValue(candidate)) continue;

                    _store.SaveId(candidate);
                    return new InstallationIdResolution(
                        candidate,
                        InstallationIdResolutionKind.Migrated,
                        source.Name);
                }

                string generatedId = _generator.CreateId();
                if (!HasValue(generatedId))
                {
                    throw new InvalidOperationException("The installation ID generator returned an empty value.");
                }

                _store.SaveId(generatedId);
                return new InstallationIdResolution(generatedId, InstallationIdResolutionKind.Generated);
            }
        }

        /// <summary>Returns the durable installation ID, resolving and saving it on first use.</summary>
        public string GetOrCreateId() => Resolve().InstallationId;

        /// <summary>
        /// Replaces the stored installation ID for an explicit account recovery or conflict-resolution flow.
        /// General consumers should use <see cref="GetOrCreateId"/>.
        /// </summary>
        public void ReplaceId(string installationId)
        {
            if (!HasValue(installationId))
                throw new ArgumentException("Installation ID must not be empty.", nameof(installationId));

            lock (_gate)
            {
                _store.SaveId(installationId);
            }
        }

        private static bool HasValue(string installationId) => !string.IsNullOrWhiteSpace(installationId);
    }
}
