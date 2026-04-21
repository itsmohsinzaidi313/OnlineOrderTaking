using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class BranchMasterMigrationService(SqlServerDbContext SqlDb) : IBranchMasterMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default)
        {
            var branches = await SqlDb.BranchMasters
                        .Where(b => b.CompanyId == companyId && b.IsActive == true)
                        .AsNoTracking()
                        .ToListAsync(ct);

            var branchDetails = await (
                        from bd in SqlDb.BranchDetails.AsNoTracking()
                        join bm in SqlDb.BranchMasters.AsNoTracking() on bd.BranchId equals bm.BranchId
                        where bd.IsActive == true && bm.IsActive == true && bm.CompanyId == companyId
                        select bd)
                        .ToListAsync(ct);

            var branchDayMappings = await (
                        from bdm in SqlDb.BranchDayMappings.AsNoTracking()
                        join bm in SqlDb.BranchMasters.AsNoTracking() on bdm.BranchId equals bm.BranchId
                        where bdm.IsActive == true && bm.IsActive == true && bm.CompanyId == companyId
                        select bdm)
                        .ToListAsync(ct);


            await PgDb.BranchMasters.ExecuteDeleteAsync(ct);
            if (branches.Count >= 1)
            {
                await PgDb.BranchMasters.AddRangeAsync(branches, ct);
            }

            await PgDb.BranchDetails.ExecuteDeleteAsync(ct);
            if (branchDetails.Count >= 1)
            {
                await PgDb.BranchDetails.AddRangeAsync(branchDetails, ct);
            }

            await PgDb.BranchDayMappings.ExecuteDeleteAsync(ct);
            if (branchDayMappings.Count >= 1)
            {
                await PgDb.BranchDayMappings.AddRangeAsync(branchDayMappings, ct);
            }
        }
    }
}
