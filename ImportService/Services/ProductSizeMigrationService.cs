using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class ProductSizeMigrationService(
        SqlServerDbContext sqlDb) : IProductSizeMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
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

