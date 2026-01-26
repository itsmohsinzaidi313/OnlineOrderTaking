using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ICityMigrationService
    {
        Task MigrateCitiesAsync(PostgresDbContext pgDb, CancellationToken ct = default);
    }
}

