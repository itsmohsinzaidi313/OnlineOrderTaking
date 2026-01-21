namespace DataMigration.Application.Interfaces
{
    public interface ISetupMasterMigrationService
    {
        Task<int> MigrateSetupMasterAsync(CancellationToken ct = default);

    }
}
