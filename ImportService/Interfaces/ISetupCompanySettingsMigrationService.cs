namespace DataMigration.Application.Interfaces
{
    public interface ISetupCompanySettingsMigrationService
    {
        Task<int> MigrateSetupCompanySettingsAsync(int companyId, CancellationToken ct = default);
    }
}
