using System;

namespace ActionFit.Identity
{
    public sealed class DelegateInstallationIdStore : InstallationIdStoreBase
    {
        private readonly Func<string> _load;
        private readonly Action<string> _save;

        public DelegateInstallationIdStore(Func<string> load, Action<string> save)
        {
            _load = load ?? throw new ArgumentNullException(nameof(load));
            _save = save ?? throw new ArgumentNullException(nameof(save));
        }

        public override string LoadId() => _load();

        public override void SaveId(string installationId) => _save(installationId);
    }

    public sealed class DelegateInstallationIdMigrationSource : InstallationIdMigrationSourceBase
    {
        private readonly Func<string> _load;

        public DelegateInstallationIdMigrationSource(string name, Func<string> load)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Migration source name must not be empty.", nameof(name));

            Name = name;
            _load = load ?? throw new ArgumentNullException(nameof(load));
        }

        public override string Name { get; }

        public override string LoadCandidate() => _load();
    }

    public sealed class GuidInstallationIdGenerator : InstallationIdGeneratorBase
    {
        public override string CreateId() => Guid.NewGuid().ToString();
    }
}
