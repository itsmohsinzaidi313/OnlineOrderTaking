using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ISetupMasterMigrationService
    {
        Task MigrateSetupMasterAsync(PostgresDbContext pgDb, CancellationToken ct = default);

    }
}
