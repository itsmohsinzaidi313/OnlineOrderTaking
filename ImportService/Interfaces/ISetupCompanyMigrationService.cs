namespace DataMigration.Application.Interfaces
{
    public interface ISetupCompanyMigrationService
    {
        Task<int> MigrateSetupCompanyAsync(int companyId,CancellationToken ct = default);
    }
}
