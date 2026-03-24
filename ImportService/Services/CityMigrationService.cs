using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class CityMigrationService(
        SqlServerDbContext sqlDb) : ICityMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {

            var cities = await sqlDb.Cities
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.Cities.ExecuteDeleteAsync(ct);
            if (cities.Count >= 1)
            {
                await pgDb.Cities.AddRangeAsync(cities, ct);
            }
        }
    }
}

