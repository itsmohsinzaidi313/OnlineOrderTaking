using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IDiscountMigrationService
    {
        Task MigrateDiscountsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}
