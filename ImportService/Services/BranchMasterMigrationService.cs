using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class BranchMasterMigrationService(SqlServerDbContext SqlDb, PostgresDbContext PgDb, ILogger<BranchMasterMigrationService> logger) : IBranchMasterMigrationService
    {
        public async Task<int> MigrateBranchMasterAsync(int companyId, CancellationToken ct = default)
        {
            logger.LogInformation("Starting Branch Migration for CompanyId={CompanyId}", companyId);

            // 1️⃣ Get all branches for the company (and optional branch)
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

            if (branches == null)
            {
                logger.LogWarning("No branch found");
                return 0;
            }

            int migratedCount = 0;

            await PgDb.BranchMasters.ExecuteDeleteAsync(ct);
            await PgDb.BranchDetails.ExecuteDeleteAsync(ct);
            await PgDb.BranchDayMappings.ExecuteDeleteAsync(ct);

            await PgDb.BranchMasters.AddRangeAsync(branches);
            await PgDb.BranchDetails.AddRangeAsync(branchDetails);
            await PgDb.BranchDayMappings.AddRangeAsync(branchDayMappings);
            migratedCount = await PgDb.SaveChangesAsync(ct);

            logger.LogInformation("✅ Migration complete. Total branches migrated: {Count}", migratedCount);
            return migratedCount;
        }
    }
}
