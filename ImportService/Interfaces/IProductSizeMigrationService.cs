namespace ImportService.Interfaces
{
    public interface IProductSizeMigrationService
    {
        Task<int> MigrateProductSizesAsync(int companyId, CancellationToken ct = default);
    }
}

