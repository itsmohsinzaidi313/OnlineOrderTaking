using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupCompanySettingsMigrationService(
        SqlServerDbContext SqlDb) : ISetupCompanySettingsMigrationService
    {

        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            var items = await SqlDb.SetupCompanySettings
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);
            if (items.Count == 0)
            {
                return;
            }
            await pgDb.SetupCompanySettings.ExecuteDeleteAsync(ct);
            await pgDb.SetupCompanySettings.AddRangeAsync(items, ct);
        }
    }
}
