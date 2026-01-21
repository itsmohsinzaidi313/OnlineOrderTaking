namespace DataMigration.Application.Interfaces
{
    public interface IOrderModeCompanyMappingMigrationService
    {
        Task<int> MigrateOrderModeCompanyMappingsAsync(int companyId, CancellationToken ct = default);
    }
}

