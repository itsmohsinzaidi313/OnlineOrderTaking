using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IGSTMigrationService
    {
        Task MigrateGSTsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}

