using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class AreaMigrationService(
        SqlServerDbContext SqlDb) : IAreaMigrationService
    {
        public async Task MigrateAreasAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default)
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

