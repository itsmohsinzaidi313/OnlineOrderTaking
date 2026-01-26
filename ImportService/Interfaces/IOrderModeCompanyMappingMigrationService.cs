using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IOrderModeCompanyMappingMigrationService
    {
        Task MigrateOrderModeCompanyMappingsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}

