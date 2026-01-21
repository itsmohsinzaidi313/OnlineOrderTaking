using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class ProductSizeMigrationService(
        SqlServerDbContext sqlDb,
        PostgresDbContext pgDb,
        ILogger<ProductSizeMigrationService> logger) : IProductSizeMigrationService
    {
        public async Task<int> MigrateProductSizesAsync(int companyId, CancellationToken ct = default)
        {
            int migrated = 0;

            try
            {
                var productSizes = await sqlDb.ProductSizes
                    .Where(x => x.IsActive == true && (x.CompanyId == null || x.CompanyId == companyId))
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (productSizes == null || productSizes.Count == 0)
                {
                    logger.LogInformation("No ProductSize rows to migrate for CompanyId={CompanyId}", companyId);
                    return 0;
                }

                await pgDb.ProductSizes.ExecuteDeleteAsync(ct);
                await pgDb.ProductSizes.AddRangeAsync(productSizes, ct);

                migrated = await pgDb.SaveChangesAsync(ct);
                logger.LogInformation("✅ ProductSize migration completed for CompanyId={CompanyId}. Rows affected: {Count}",
                    companyId, migrated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error migrating ProductSizes for CompanyId={CompanyId}", companyId);
                throw;
            }

            return migrated;
        }
    }
}

