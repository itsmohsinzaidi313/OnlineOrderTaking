using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IFlavourMigrationService
    {
        Task MigrateFlavoursAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}

