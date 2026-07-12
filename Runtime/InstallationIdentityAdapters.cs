using System;

namespace ActionFit.Identity
{
    public sealed class DelegateInstallationIdStore : IInstallationIdStore
    {
        private readonly Func<string> _load;
        private readonly Action<string> _save;

        public DelegateInstallationIdStore(Func<string> load, Action<string> save)
        {
            _load = load ?? throw new ArgumentNullException(nameof(load));
            _save = save ?? throw new ArgumentNullException(nameof(save));
        }

        public string LoadId() => _load();

        public void SaveId(string installationId) => _save(installationId);
    }

    public sealed class DelegateInstallationIdMigrationSource : IInstallationIdMigrationSource
    {
        private readonly Func<string> _load;

        public DelegateInstallationIdMigrationSource(string name, Func<string> load)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Migration source name must not be empty.", nameof(name));

            Name = name;
            _load = load ?? throw new ArgumentNullException(nameof(load));
        }

        public string Name { get; }

        public string LoadCandidate() => _load();
    }

    public sealed class GuidInstallationIdGenerator : IInstallationIdGenerator
    {
        public string CreateId() => Guid.NewGuid().ToString();
    }
}
