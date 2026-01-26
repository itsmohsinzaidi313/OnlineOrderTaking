using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImportService.Services
{
    public class SetupMasterMigrationService(SqlServerDbContext sqlDb) : ISetupMasterMigrationService
    {
        public async Task MigrateSetupMasterAsync(PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var masters = await sqlDb.SetupMasters
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.SetupMasters.ExecuteDeleteAsync(ct);
            if(masters.Count == 0)
            {
                return;
            }
            await pgDb.SetupMasters.AddRangeAsync(masters, ct);
        }

    }
}
