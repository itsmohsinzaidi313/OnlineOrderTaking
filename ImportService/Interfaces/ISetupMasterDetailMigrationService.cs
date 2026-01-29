using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ISetupMasterDetailMigrationService
    {
        Task MigrateSetupMasterDetailAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}
