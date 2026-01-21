namespace DataMigration.Application.Interfaces
{
    public interface ISetupMasterDetailMigrationService
    {
        Task<int> MigrateSetupMasterDetailAsync(int companyId, CancellationToken ct = default);
    }
}
