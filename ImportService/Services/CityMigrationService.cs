using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class CityMigrationService(
        SqlServerDbContext sqlDb,
        PostgresDbContext pgDb,
        ILogger<CityMigrationService> logger) : ICityMigrationService
    {
        public async Task<int> MigrateCitiesAsync(CancellationToken ct = default)
        {
            int migrated = 0;

            try
            {
                var cities = await sqlDb.Cities
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (cities == null || cities.Count == 0)
                {
                    logger.LogInformation("No City rows to migrate");
                    return 0;
                }
                await pgDb.Cities.ExecuteDeleteAsync(ct);
                await pgDb.Cities.AddRangeAsync(cities, ct);

                migrated = await pgDb.SaveChangesAsync(ct);
                logger.LogInformation("✅ City migration completed. Rows affected: {Count}", migrated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error migrating Cities");
                throw;
            }

            return migrated;
        }
    }
}

