using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class SetupCompanySettingsMigrationService(
        SqlServerDbContext SqlDb,
        PostgresDbContext PgDb,
        ILogger<SetupCompanySettingsMigrationService> Logger) : ISetupCompanySettingsMigrationService
    {

        public async Task<int> MigrateSetupCompanySettingsAsync(int companyId, CancellationToken ct = default)
        {
            try
            {
                var items = await SqlDb.SetupCompanySettings
                    .Where(x => x.CompanyId == null || x.CompanyId == companyId)
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (items == null || items.Count == 0)
                {
                    Logger.LogInformation("No SetupCompanySetting rows to migrate for CompanyId={CompanyId}", companyId);
                    return 0;
                }

                int migrated = 0;
                await PgDb.SetupCompanySettings.ExecuteDeleteAsync(ct);
                await PgDb.SetupCompanySettings.AddRangeAsync(items, ct);
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

                var saved = await PgDb.SaveChangesAsync(ct);
                Logger.LogInformation("✅ SetupCompanySetting migration completed for CompanyId={CompanyId}. Rows affected: {Count}", companyId, saved);
                return saved;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ Error migrating SetupCompanySettings for CompanyId={CompanyId}", companyId);
                throw;
            }
        }
    }
}
