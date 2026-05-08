using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class CityMigrationService(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory) : ICityMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
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

