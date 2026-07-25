namespace ActionFit.Identity
{
    public abstract class InstallationIdStoreBase
    {
        public abstract string LoadId();
        public abstract void SaveId(string installationId);
    }

    public abstract class InstallationIdMigrationSourceBase
    {
        public abstract string Name { get; }
        public abstract string LoadCandidate();
    }

    public abstract class InstallationIdGeneratorBase
    {
        public abstract string CreateId();
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
