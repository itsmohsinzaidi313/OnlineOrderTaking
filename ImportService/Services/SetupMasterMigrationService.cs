using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupMasterMigrationService(SqlServerDbContext sqlDb) : ISetupMasterMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
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
