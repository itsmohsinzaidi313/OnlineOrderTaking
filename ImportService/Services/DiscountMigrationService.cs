using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class DiscountMigrationService(
        SqlServerDbContext sqlDb,
        PostgresDbContext pgDb,
        ILogger<DiscountMigrationService> logger) : IDiscountMigrationService
    {
        public async Task<int> MigrateDiscountsAsync(int companyId, CancellationToken ct = default)
        {
            int migrated = 0;

            try
            {
                // 1) Migrate main Discount records for this company
                var discounts = await sqlDb.Discounts
                    .Where(x => x.IsActive == true && x.CompanyId == companyId)
                    .AsNoTracking()
                    .ToListAsync(ct);

                await pgDb.Discounts.ExecuteDeleteAsync(ct);
                await pgDb.Discounts.AddRangeAsync(discounts, ct);

                var discountIds = discounts.Select(x => x.DiscountId).ToHashSet();

                // 2) Migrate DiscountDayMappings for the migrated discounts
                var dayMappings = await sqlDb.DiscountDayMappings
                    .Where(x => discountIds.Contains(x.DiscountId) && x.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync(ct);
                await pgDb.DiscountDayMappings.ExecuteDeleteAsync(ct);
                await pgDb.DiscountDayMappings.AddRangeAsync(dayMappings, ct);



                // 3) Migrate DiscountBranchMappings for the migrated discounts
                var branchMappings = await sqlDb.DiscountBranchMappings
                    .Where(x => discountIds.Contains(x.DiscountId) && x.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync(ct);

                await pgDb.DiscountBranchMappings.ExecuteDeleteAsync(ct);
                await pgDb.DiscountBranchMappings.AddRangeAsync(branchMappings, ct);

                // 4) Migrate DiscountOrderTypeMappings for the migrated discounts
                var orderTypeMappings = await sqlDb.DiscountOrderTypeMappings
                    .Where(x => discountIds.Contains(x.DiscountId) && x.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync(ct);

                await pgDb.DiscountOrderTypeMappings.ExecuteDeleteAsync(ct);
                await pgDb.DiscountOrderTypeMappings.AddRangeAsync(orderTypeMappings, ct);

                // 5) Migrate DiscountOrderModeMappings for the migrated discounts
                var orderModeMappings = await sqlDb.DiscountOrderModeMappings
                    .Where(x => discountIds.Contains(x.DiscountId) && x.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync(ct);

                await pgDb.DiscountOrderModeMappings.ExecuteDeleteAsync(ct);
                await pgDb.DiscountOrderModeMappings.AddRangeAsync(orderModeMappings, ct);

                // 6) Migrate DiscountProductDetailMappings for the migrated discounts
                var productDetailMappings = await sqlDb.DiscountProductDetailMappings
                    .Where(x => discountIds.Contains(x.DiscountId) && x.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync(ct);

                await pgDb.DiscountProductDetailMappings.ExecuteDeleteAsync(ct);
                await pgDb.DiscountProductDetailMappings.AddRangeAsync(productDetailMappings, ct);

                migrated = await pgDb.SaveChangesAsync(ct);
                logger.LogInformation("✅ Discount migration completed for CompanyId={CompanyId}. Rows affected: {Count}",
                    companyId, migrated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error migrating discounts for CompanyId={CompanyId}", companyId);
                throw;
            }

            return migrated;
        }
    }
}
