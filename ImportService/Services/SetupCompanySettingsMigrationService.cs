using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupCompanySettingsMigrationService(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory) : ISetupCompanySettingsMigrationService
    {

        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var items = await sqlDb.SetupCompanySettings
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
