namespace ActionFit.Identity
{
    public interface IInstallationIdStore
    {
        string LoadId();
        void SaveId(string installationId);
    }

    public interface IInstallationIdMigrationSource
    {
        string Name { get; }
        string LoadCandidate();
    }

    public interface IInstallationIdGenerator
    {
        string CreateId();
    }

    public enum InstallationIdResolutionKind
    {
        Stored,
        Migrated,
        Generated
    }

    public readonly struct InstallationIdResolution
    {
        public InstallationIdResolution(
            string installationId,
            InstallationIdResolutionKind kind,
            string migrationSource = "")
        {
            InstallationId = installationId;
            Kind = kind;
            MigrationSource = migrationSource ?? "";
        }

        public string InstallationId { get; }
        public InstallationIdResolutionKind Kind { get; }
        public string MigrationSource { get; }
    }
}
