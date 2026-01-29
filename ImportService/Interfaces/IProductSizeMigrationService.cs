using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IProductSizeMigrationService
    {
        Task MigrateProductSizesAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}

