using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class BranchMasterMigrationService(SqlServerDbContext SqlDb) : IBranchMasterMigrationService
    {
        public async Task MigrateBranchMasterAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default)
        {
            var branches = await SqlDb.BranchMasters
                        .Where(b => b.CompanyId == companyId && b.IsActive == true)
                        .AsNoTracking()
                        .ToListAsync(ct);

            var branchIds = branches.Select(b => b.BranchId).ToList();

            var branchDetails = await SqlDb.BranchDetails
                        .Join(SqlDb.BranchMasters, a => a.BranchId, b => b.BranchId, (a, b) => a)
                        .Where(a => a.IsActive == true && branchIds.Contains(a.BranchId))
                        .AsNoTracking()
                        .ToListAsync(ct);

            var branchDayMappings = await SqlDb.BranchDayMappings.Where(a => a.IsActive == true)
                        .Join(SqlDb.BranchMasters, a => a.BranchId, b => b.BranchId, (a, b) => a)
                        .Where(b => branchIds.Contains(b.BranchId) && b.IsActive == true)
                        .AsNoTracking()
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
