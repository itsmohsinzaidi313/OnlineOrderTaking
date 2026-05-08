using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class RidersMigrationService(IDbContextFactory<SqlServerDbContext> sqlDbFactory) : IRidersMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var riders = await sqlDb.Riders
                .Where(x => x.IsActive == true && x.BranchId != null && x.RiderName != null)
                .Join(sqlDb.BranchMasters, a => a.BranchId, b => b.BranchId, (a, b) => new { Rider = a, Branch = b })
                .Where(x => x.Branch.CompanyId == companyId)
                .Select(x => x.Rider)
                .ToListAsync(ct);

            await pgDb.Riders.ExecuteDeleteAsync(ct);
            await pgDb.Riders.AddRangeAsync(riders, ct);
        }
    }
}
