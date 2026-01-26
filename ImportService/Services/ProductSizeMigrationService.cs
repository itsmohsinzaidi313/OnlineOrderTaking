using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class ProductSizeMigrationService(
        SqlServerDbContext sqlDb) : IProductSizeMigrationService
    {
        public async Task MigrateProductSizesAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var productSizes = await sqlDb.ProductSizes
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductSizes.ExecuteDeleteAsync(ct);
            await pgDb.ProductSizes.AddRangeAsync(productSizes, ct);
        }
    }
}

