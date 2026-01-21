namespace ImportService.Interfaces
{
    public interface ISetupCompanySettingsMigrationService
    {
        Task<int> MigrateSetupCompanySettingsAsync(int companyId, CancellationToken ct = default);
    }
}
