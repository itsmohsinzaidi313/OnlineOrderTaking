using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IMigrationService
    {
        Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default);
    }
}
