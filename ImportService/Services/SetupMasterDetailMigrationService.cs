using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class SetupMasterDetailMigrationService(
        SqlServerDbContext sqlDb) : ISetupMasterDetailMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            var details = await sqlDb.SetupMasterDetails
                .Where(d => (d.CompanyId == null || d.CompanyId == companyId) && d.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);
            
            await pgDb.SetupMasterDetails.ExecuteDeleteAsync(ct);
            await pgDb.SetupMasterDetails.AddRangeAsync(details, ct);

            var statuses = await sqlDb.OrderStatuses
                .Where(x => x.CompanyId == companyId && x.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.OrderStatuses.ExecuteDeleteAsync(ct);
            await pgDb.AddRangeAsync(statuses, ct);
        }
    }
}


