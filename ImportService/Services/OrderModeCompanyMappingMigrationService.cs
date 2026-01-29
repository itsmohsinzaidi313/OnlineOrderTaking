using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class OrderModeCompanyMappingMigrationService(SqlServerDbContext sqlDb) : IOrderModeCompanyMappingMigrationService
    {
        public async Task MigrateOrderModeCompanyMappingsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var source = await sqlDb.OrderModeCompanyMappings
                .Where(x => x.CompanyId == companyId && x.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.OrderModeCompanyMappings.ExecuteDeleteAsync(ct);
            await pgDb.OrderModeCompanyMappings.AddRangeAsync(source, ct);
        }
    }
}

