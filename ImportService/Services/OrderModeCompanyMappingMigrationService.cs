using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class OrderModeCompanyMappingMigrationService(SqlServerDbContext sqlDb) : IOrderModeCompanyMappingMigrationService
    {
        public async Task MigrateAsync( PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            var source = await sqlDb.OrderModeCompanyMappings
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.OrderModeCompanyMappings.ExecuteDeleteAsync(ct);
            await pgDb.OrderModeCompanyMappings.AddRangeAsync(source, ct);
        }
    }
}

