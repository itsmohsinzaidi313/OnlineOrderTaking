using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ISetupCompanyMigrationService
    {
        Task MigrateSetupCompanyAsync(int companyId,  PostgresDbContext pgDb, CancellationToken ct = default);
    }
}
