namespace DataMigration.Application.Interfaces
{
    public interface IDiscountMigrationService
    {
        Task<int> MigrateDiscountsAsync(int companyId, CancellationToken ct = default);
    }
}
