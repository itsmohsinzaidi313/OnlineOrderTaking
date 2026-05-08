using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class ProductSizeMigrationService(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory) : IProductSizeMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var productSizes = await sqlDb.ProductSizes
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductSizes.ExecuteDeleteAsync(ct);
            await pgDb.ProductSizes.AddRangeAsync(productSizes, ct);
        }
    }
}

