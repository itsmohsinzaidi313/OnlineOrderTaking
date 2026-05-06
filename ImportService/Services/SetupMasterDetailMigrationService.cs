using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupMasterDetailMigrationService(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory) : ISetupMasterDetailMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
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


