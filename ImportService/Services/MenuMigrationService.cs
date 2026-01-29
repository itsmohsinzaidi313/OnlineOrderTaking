using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class MenuMigrationService(SqlServerDbContext SqlDb) : IMenuMigrationService
    {
        public async Task MigrateMenuAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            // 3) Categories
            var categories = await SqlDb.ProductCategories
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductCategories.ExecuteDeleteAsync(ct);
            await pgDb.ProductCategories.AddRangeAsync(categories, ct);

            // Pre-compute relevant category ids for this company
            var categoryIds = categories.Select(c => c.CategoryId).ToHashSet();

            // 3.1) CategoryAvailability (by categories of this company)
            var categoryAvailabilities = await SqlDb.CategoryAvailabilities
                .Where(ca => ca.IsActive == true && categoryIds.Contains(ca.CategoryId ?? 0))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.CategoryAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.CategoryAvailabilities.AddRangeAsync(categoryAvailabilities, ct);

            // 4) Products (by categories of this company)
            var products = await SqlDb.Products
                .Where(p => p.IsActive == true && categoryIds.Contains(p.ProductCategoryId ?? 0))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.Products.ExecuteDeleteAsync(ct);
            await pgDb.Products.AddRangeAsync(products, ct);

            var productIds = products.Select(x => x.ProductId).ToHashSet();

            // 5) ProductDetails (by products of this company)
            var details = await SqlDb.ProductDetails
                .Where(d => d.IsActive == true && productIds.Contains(d.ProductId))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetails.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetails.AddRangeAsync(details, ct);

            // 5.1) Get branch IDs for this company
            var branchIds = await SqlDb.BranchMasters
                .Where(b => b.CompanyId == companyId && b.IsActive == true)
                .Select(b => b.BranchId)
                .ToListAsync(ct);
            var branchIdsSet = branchIds.ToHashSet();

            // 6) ProductDetailBranchMapping (for the migrated product details and company branches)
            var productDetailIds = details.Select(x => x.ProductDetailId).ToHashSet();

            var productDetailBranchMappings = await SqlDb.ProductDetailBranchMappings
                .Where(pdbm => productDetailIds.Contains(pdbm.ProductDetailId!.Value) &&
                               branchIdsSet.Contains(pdbm.BranchId.Value))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailBranchMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailBranchMappings.AddRangeAsync(productDetailBranchMappings, ct);

            // 6.1) ProductDetailAvailability (for the migrated ProductDetailBranchMappings)
            var productBranchMappingIds = productDetailBranchMappings.Select(x => x.ProductDetailBranchMappingId).ToHashSet();

            var productDetailAvailabilities = await SqlDb.ProductDetailAvailabilities
                .Where(pda => productBranchMappingIds.Contains(pda.ProductBranchId.Value) &&
                              pda.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailAvailabilities.AddRangeAsync(productDetailAvailabilities, ct);

            // 6.2) ProductDetailOrderSourcePriceMapping (for the migrated product details)
            var productDetailOrderSourcePriceMappings = await SqlDb.ProductDetailOrderSourcePriceMappings
                .Where(pdosm => productDetailIds.Contains(pdosm.ProductDetailId.Value) &&
                                pdosm.IsActive == true &&
                                branchIdsSet.Contains(pdosm.BranchId.Value))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductDetailOrderSourcePriceMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailOrderSourcePriceMappings.AddRangeAsync(productDetailOrderSourcePriceMappings, ct);

            // 7) DealItemDetails (for the migrated product details)

            var dealItems = await SqlDb.DealItemDetails
                .Where(di => di.IsActive == true && productDetailIds.Contains(di.ProductDetailId))
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DealItemDetails.ExecuteDeleteAsync(ct);
            await pgDb.DealItemDetails.AddRangeAsync(dealItems, ct);

            // 7) DealDescriptions (linked via DealItemId or ProductDetailId)
            var dealItemIds = dealItems.Select(x => x.DealItemId).ToHashSet();

            var dealDescriptions = await SqlDb.DealDescriptions
                .Where(dd => dd.IsActive == true && dealItemIds.Contains(dd.DealItemId.Value)
                    )
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DealDescriptions.ExecuteDeleteAsync(ct);
            await pgDb.DealDescriptions.AddRangeAsync(dealDescriptions, ct);
        }
    }
}


