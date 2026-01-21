namespace ImportService.Interfaces
{
    public interface IOrderModeCompanyMappingMigrationService
    {
        Task<int> MigrateOrderModeCompanyMappingsAsync(int companyId, CancellationToken ct = default);
    }
}

