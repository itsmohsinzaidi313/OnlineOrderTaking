using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class MenuMigrationService(SqlServerDbContext sqlDb, PostgresDbContext pgDb, ILogger<MenuMigrationService> logger) : IMenuMigrationService
    {
        public async Task<int> MigrateMenuAsync(int companyId, CancellationToken ct = default)
        {
            int migrated = 0;

            // 3) Categories
            var categories = await sqlDb.ProductCategories
                .Where(x => x.IsActive == true && x.CompanyId == companyId || x.CompanyId == null)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductCategories.ExecuteDeleteAsync(ct);
            await pgDb.ProductCategories.AddRangeAsync(categories, ct);

            // Pre-compute relevant category ids for this company
            var categoryIds = categories.Select(c => c.CategoryId).ToHashSet();

            // 3.1) CategoryAvailability (by categories of this company)
            var categoryAvailabilities = await sqlDb.CategoryAvailabilities
                .Where(ca => ca.IsActive == true && ca.CategoryId != null && categoryIds.Contains(ca.CategoryId.Value))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.CategoryAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.CategoryAvailabilities.AddRangeAsync(categoryAvailabilities, ct);

            // 4) Products (by categories of this company)
            var products = await sqlDb.Products
                .Where(p => //p.ProductId == 9503 && 
                p.IsActive == true && (p.ProductCategoryId == null || categoryIds.Contains(p.ProductCategoryId.Value)))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.Products.ExecuteDeleteAsync(ct);
            await pgDb.Products.AddRangeAsync(products, ct);

            var productIds = products.Select(x => x.ProductId).ToHashSet();

            // 5) ProductDetails (by products of this company)
            var details = await sqlDb.ProductDetails
                .Where(d => d.IsActive == true && productIds.Contains(d.ProductId))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetails.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetails.AddRangeAsync(details, ct);

            // 5.1) Get branch IDs for this company
            var branchIds = await sqlDb.BranchMasters
                .Where(b => b.CompanyId == companyId && b.IsActive == true)
                .Select(b => b.BranchId)
                .ToListAsync(ct);
            var branchIdsSet = branchIds.ToHashSet();

            // 6) ProductDetailBranchMapping (for the migrated product details and company branches)
            var productDetailIds = details.Select(x => x.ProductDetailId).ToHashSet();

            var productDetailBranchMappings = await sqlDb.ProductDetailBranchMappings
                .Where(pdbm => productDetailIds.Contains(pdbm.ProductDetailId!.Value) &&
                               (pdbm.BranchId == null || branchIdsSet.Contains(pdbm.BranchId.Value)))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailBranchMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailBranchMappings.AddRangeAsync(productDetailBranchMappings, ct);

            // 6.1) ProductDetailAvailability (for the migrated ProductDetailBranchMappings)
            var productBranchMappingIds = productDetailBranchMappings.Select(x => x.ProductDetailBranchMappingId).ToHashSet();

            var productDetailAvailabilities = await sqlDb.ProductDetailAvailabilities
                .Where(pda => pda.ProductBranchId != null && productBranchMappingIds.Contains(pda.ProductBranchId.Value) &&
                              pda.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailAvailabilities.AddRangeAsync(productDetailAvailabilities, ct);

            // 6.2) ProductDetailOrderSourcePriceMapping (for the migrated product details)
            var productDetailOrderSourcePriceMappings = await sqlDb.ProductDetailOrderSourcePriceMappings
                .Where(pdosm => productDetailIds.Contains(pdosm.ProductDetailId.Value) &&
                                pdosm.IsActive == true &&
                                (pdosm.BranchId == null || branchIdsSet.Contains(pdosm.BranchId.Value)))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailOrderSourcePriceMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailOrderSourcePriceMappings.AddRangeAsync(productDetailOrderSourcePriceMappings, ct);

            // 7) DealItemDetails (for the migrated product details)

            var dealItems = await sqlDb.DealItemDetails
                .Where(di => di.IsActive == true && productDetailIds.Contains(di.ProductDetailId)
                )
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DealItemDetails.ExecuteDeleteAsync(ct);
            await pgDb.DealItemDetails.AddRangeAsync(dealItems, ct);

            // 7) DealDescriptions (linked via DealItemId or ProductDetailId)
            var dealItemIds = dealItems.Select(x => x.DealItemId).ToHashSet();

            var dealDescriptions = await sqlDb.DealDescriptions
                .Where(dd => dd.IsActive == true &&
                    (dd.DealItemId != null && dealItemIds.Contains(dd.DealItemId.Value))
                    )
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DealDescriptions.ExecuteDeleteAsync(ct);
            await pgDb.DealDescriptions.AddRangeAsync(dealDescriptions, ct);

            migrated += await pgDb.SaveChangesAsync(ct);
            logger.LogInformation("✅ Menu migration completed for CompanyId={CompanyId}. Rows affected: {Count}", companyId, migrated);
            return migrated;
        }
    }
}


