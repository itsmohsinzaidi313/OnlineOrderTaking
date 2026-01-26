using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ISetupCompanySettingsMigrationService
    {
        Task MigrateSetupCompanySettingsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}
