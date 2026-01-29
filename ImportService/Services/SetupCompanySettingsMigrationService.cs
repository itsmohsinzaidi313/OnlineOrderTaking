using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupCompanySettingsMigrationService(
        SqlServerDbContext SqlDb) : ISetupCompanySettingsMigrationService
    {

        public async Task MigrateSetupCompanySettingsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
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
            //foreach (var item in items)
            //{
            //    var existing = await PgDb.SetupCompanySettings
            //        .AsNoTracking()
            //        .FirstOrDefaultAsync(x => x.SettingId == item.SettingId, ct);

            //    var obj = new SetupCompanySetting
            //    {
            //        SettingId = item.SettingId,
            //        SetupDetailId = item.SetupDetailId,
            //        SettingValue = item.SettingValue,
            //        IsActive = item.IsActive,
            //        CreatedBy = item.CreatedBy,
            //        CreatedDate = item.CreatedDate,
            //        ModifiedBy = item.ModifiedBy,
            //        ModifiedDate = item.ModifiedDate,
            //        UserIP = item.UserIP,
            //        CompanyId = item.CompanyId,
            //        BranchId = item.BranchId
            //    };

            //    if (existing == null)
            //    {
            //        PgDb.SetupCompanySettings.Add(obj);
            //        Logger.LogInformation("Inserted SetupCompanySetting {SettingId}", item.SettingId);
            //    }
            //    else
            //    {
            //        PgDb.SetupCompanySettings.Attach(existing);
            //        PgDb.Entry(existing).CurrentValues.SetValues(obj);
            //        Logger.LogInformation("Updated SetupCompanySetting {SettingId}", item.SettingId);
            //    }

            //    migrated++;
            //}

        }
    }
}
