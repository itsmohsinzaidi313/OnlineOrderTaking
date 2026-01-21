using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupMasterDetailMigrationService(
        SqlServerDbContext sqlDb,
        PostgresDbContext pgDb,
        ILogger<SetupMasterDetailMigrationService> logger) : ISetupMasterDetailMigrationService
    {
        public async Task<int> MigrateSetupMasterDetailAsync(int companyId, CancellationToken ct = default)
        {
            var details = await sqlDb.SetupMasterDetails
                .Where(d => d.CompanyId == companyId && d.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            if (details == null || details.Count == 0)
            {
                logger.LogWarning("No active SetupMasterDetail rows found to migrate for CompanyId={CompanyId} (including nulls)", companyId);
                return 0;
            }

            int migratedCount = 0;
            await pgDb.SetupMasterDetails.ExecuteDeleteAsync(ct);
            await pgDb.SetupMasterDetails.AddRangeAsync(details, ct);

            await pgDb.SaveChangesAsync(ct);
            return migratedCount;
        }
    }
}


