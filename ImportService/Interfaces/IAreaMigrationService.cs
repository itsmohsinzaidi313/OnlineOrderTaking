using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IAreaMigrationService
    {
        Task MigrateAreasAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default);
    }
}

