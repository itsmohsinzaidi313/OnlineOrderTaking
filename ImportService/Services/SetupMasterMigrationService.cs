using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupMasterMigrationService(
        SqlServerDbContext sqlDb,
        PostgresDbContext pgDb,
        ILogger<SetupMasterMigrationService> logger) : ISetupMasterMigrationService
    {
        public async Task<int> MigrateSetupMasterAsync(CancellationToken ct = default)
        {
            var masters = await sqlDb.SetupMasters
                .AsNoTracking()
                .ToListAsync(ct);

            if (masters == null || masters.Count == 0)
            {
                logger.LogWarning("No SetupMaster rows found to migrate");
                return 0;
            }

            await pgDb.SetupMasters.ExecuteDeleteAsync(ct);
            await pgDb.SetupMasters.AddRangeAsync(masters, ct);
            int migratedCount = await pgDb.SaveChangesAsync(ct);
            return migratedCount;
        }

    }
}
