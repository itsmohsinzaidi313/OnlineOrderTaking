using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class MenuMigrationService(SqlServerDbContext SqlDb) : IMenuMigrationService
    {
        public async Task MigrateAsync( PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            // 3) Categories
            var categories = await SqlDb.ProductCategories
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.ProductCategories.ExecuteDeleteAsync(ct);
            await pgDb.ProductCategories.AddRangeAsync(categories, ct);

            // 3.1) CategoryAvailability (by categories of this company)
            var categoryAvailabilities = await (
                    from ca in SqlDb.CategoryAvailabilities.AsNoTracking()
                    join c in SqlDb.ProductCategories.AsNoTracking() on ca.CategoryId equals c.CategoryId
                    where ca.IsActive == true && c.CompanyId == companyId
                    select ca)
                .ToListAsync(ct);

            await pgDb.CategoryAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.CategoryAvailabilities.AddRangeAsync(categoryAvailabilities, ct);

            // 4) Products (by categories of this company)
            var products = await (
                    from p in SqlDb.Products.AsNoTracking()
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    where (p.IsActive == true && p.DisplayInWeb == true) && c.CompanyId == companyId
                    select p)
                .ToListAsync(ct);

            await pgDb.Products.ExecuteDeleteAsync(ct);
            await pgDb.Products.AddRangeAsync(products, ct);

            // 5) ProductDetails (by products of this company)
            var details = await (
                    from d in SqlDb.ProductDetails.AsNoTracking()
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    where d.IsActive == true && (p.IsActive == true && p.DisplayInWeb == true) && c.IsActive == true && c.CompanyId == companyId
                    select d)
                .ToListAsync(ct);

            await pgDb.ProductDetails.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetails.AddRangeAsync(details, ct);

            // 6) ProductDetailBranchMapping (for the migrated product details and company branches)
            var productDetailBranchMappings = await (
                    from pdbm in SqlDb.ProductDetailBranchMappings.AsNoTracking()
                    where pdbm.ProductDetailId != null && pdbm.BranchId != null
                    join d in SqlDb.ProductDetails.AsNoTracking() on pdbm.ProductDetailId equals d.ProductDetailId
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    join b in SqlDb.BranchMasters.AsNoTracking() on pdbm.BranchId equals b.BranchId
                    where d.IsActive == true && p.IsActive == true && c.IsActive == true &&
                          b.IsActive == true && c.CompanyId == companyId && b.CompanyId == companyId
                    select pdbm)
                .ToListAsync(ct);

            await pgDb.ProductDetailBranchMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailBranchMappings.AddRangeAsync(productDetailBranchMappings, ct);

            // 6.1) ProductDetailAvailability (for the migrated ProductDetailBranchMappings)
            var productDetailAvailabilities = await (
                    from pda in SqlDb.ProductDetailAvailabilities.AsNoTracking()
                    join pdbm in SqlDb.ProductDetailBranchMappings.AsNoTracking() on pda.ProductBranchId equals pdbm.ProductDetailBranchMappingId
                    where pda.IsActive == true && pdbm.ProductDetailId != null && pdbm.BranchId != null
                    join d in SqlDb.ProductDetails.AsNoTracking() on pdbm.ProductDetailId equals d.ProductDetailId
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    join b in SqlDb.BranchMasters.AsNoTracking() on pdbm.BranchId equals b.BranchId
                    where d.IsActive == true && p.IsActive == true && c.IsActive == true &&
                          b.IsActive == true && c.CompanyId == companyId && b.CompanyId == companyId
                    select pda)
                .ToListAsync(ct);

            await pgDb.ProductDetailAvailabilities.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailAvailabilities.AddRangeAsync(productDetailAvailabilities, ct);

            // 6.2) ProductDetailOrderSourcePriceMapping (for the migrated product details)
            var productDetailOrderSourcePriceMappings = await (
                    from pdosm in SqlDb.ProductDetailOrderSourcePriceMappings.AsNoTracking()
                    join d in SqlDb.ProductDetails.AsNoTracking() on pdosm.ProductDetailId equals d.ProductDetailId
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    join b in SqlDb.BranchMasters.AsNoTracking() on pdosm.BranchId equals b.BranchId
                    where pdosm.IsActive == true && d.IsActive == true && p.IsActive == true &&
                          c.IsActive == true && b.IsActive == true && c.CompanyId == companyId && b.CompanyId == companyId
                    select pdosm)
                .ToListAsync(ct);

            await pgDb.ProductDetailOrderSourcePriceMappings.ExecuteDeleteAsync(ct);
            await pgDb.ProductDetailOrderSourcePriceMappings.AddRangeAsync(productDetailOrderSourcePriceMappings, ct);

            // 7) DealItemDetails (for the migrated product details)

            var dealItems = await (
                    from di in SqlDb.DealItemDetails.AsNoTracking()
                    join d in SqlDb.ProductDetails.AsNoTracking() on di.ProductDetailId equals d.ProductDetailId
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    where di.IsActive == true && d.IsActive == true && p.IsActive == true && c.IsActive == true && c.CompanyId == companyId
                    select di)
                .ToListAsync(ct);

            await pgDb.DealItemDetails.ExecuteDeleteAsync(ct);
            await pgDb.DealItemDetails.AddRangeAsync(dealItems, ct);

            // 7) DealDescriptions (linked via DealItemId or ProductDetailId)
            var dealDescriptions = await (
                    from dd in SqlDb.DealDescriptions.AsNoTracking()
                    join di in SqlDb.DealItemDetails.AsNoTracking() on dd.DealItemId equals di.DealItemId
                    join d in SqlDb.ProductDetails.AsNoTracking() on di.ProductDetailId equals d.ProductDetailId
                    join p in SqlDb.Products.AsNoTracking() on d.ProductId equals p.ProductId
                    join c in SqlDb.ProductCategories.AsNoTracking() on p.ProductCategoryId equals c.CategoryId
                    where dd.IsActive == true && di.IsActive == true && d.IsActive == true &&
                          p.IsActive == true && c.IsActive == true && c.CompanyId == companyId
                    select dd)
                .ToListAsync(ct);

            await pgDb.DealDescriptions.ExecuteDeleteAsync(ct);
            await pgDb.DealDescriptions.AddRangeAsync(dealDescriptions, ct);
        }
    }
}


