using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class AreaMigrationService(
        SqlServerDbContext SqlDb) : IAreaMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default)
        {
            var areas = await SqlDb.Areas
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await PgDb.Areas.ExecuteDeleteAsync(ct);
            if(areas.Count == 0)
            {
                return;
            }
            await PgDb.Areas.AddRangeAsync(areas, ct);
        }
    }
}

