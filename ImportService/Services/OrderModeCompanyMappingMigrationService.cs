using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class OrderModeCompanyMappingMigrationService(SqlServerDbContext sqlDb, PostgresDbContext pgDb, ILogger<OrderModeCompanyMappingMigrationService> logger) : IOrderModeCompanyMappingMigrationService
    {
        public async Task<int> MigrateOrderModeCompanyMappingsAsync(int companyId, CancellationToken ct = default)
        {
            var source = await sqlDb.OrderModeCompanyMappings
                .Where(x => x.CompanyId == companyId && x.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            if (source == null || source.Count == 0)
            {
                logger.LogInformation("No OrderModeCompanyMapping rows to migrate for CompanyId={CompanyId}", companyId);
                return 0;
            }

            int migrated = 0;

            await pgDb.OrderModeCompanyMappings.ExecuteDeleteAsync(ct);
            await pgDb.AddRangeAsync(source, ct);

            await pgDb.SaveChangesAsync(ct);
            logger.LogInformation("Migrated {Count} OrderModeCompanyMapping rows for CompanyId={CompanyId}", migrated, companyId);
            return migrated;
        }
    }
}

