using Db = PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Application;
using Microsoft.Extensions.DependencyInjection;

namespace GetMenuService;

internal class Implementation(IServiceProvider service, Db.PgDbContext dbContext)
{
    internal async IAsyncEnumerable<Category> GetMenuAsync(int companyId, int branchId = 0)
    {
        var dbSizes = new List<Db.ProductSize>();
        var dbFlavours = new List<Db.Flavour>();
        var dbProducts = new List<Db.Product>();
        var dbProductDetails = new List<Db.ProductDetail>();
        var dbDealItemDetails = new List<Db.DealItemDetail>();
        var dbDealDescription = new List<Db.DealDescription>();
        var dbDealDescriptionProducts = new List<Db.Product>();
        var dbDepartments = new Dictionary<int, string>();
        var dbDiscounts = new List<Db.Discount>();

        var tasks = new List<Task>
    {
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbSizes.AddRange(from a in dbContext.ProductSizes
                             where a.CompanyId == companyId
                             select a);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbFlavours.AddRange(from a in dbContext.Flavours
                                where a.CompanyId == companyId
                                select a);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbProducts.AddRange(from a in dbContext.Products
                                join b in dbContext.ProductCategories on a.ProductCategoryId equals b.CategoryId
                                where b.CompanyId == companyId
                                select a);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbDealItemDetails.AddRange(from a in dbContext.DealItemDetails
                                       join c in dbContext.ProductDetails on a.ProductDetailId equals c.ProductDetailId
                                       join d in dbContext.Products on c.ProductId equals d.ProductId
                                       join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                       where e.CompanyId == companyId
                                       select a);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbDealDescription.AddRange(from a in dbContext.DealDescriptions
                                       join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                       join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                       join d in dbContext.Products on c.ProductId equals d.ProductId
                                       join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                       where e.CompanyId == companyId
                                       select a);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbDealDescriptionProducts.AddRange(from a in dbContext.DealDescriptions
                                               join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                               join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                               join d in dbContext.Products on c.ProductId equals d.ProductId
                                               join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                               join f in dbContext.ProductDetails on a.ProductDetailId equals f.ProductDetailId
                                               join g in dbContext.Products on f.ProductId equals g.ProductId
                                               where e.CompanyId == companyId
                                               select g);
        }),
        Task.Run(() =>
        {
            using var scope = service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
            dbDepartments = (from a in dbContext.ProductCategories
                            select new { a.CategoryId, a.DepartmentId }).ToDictionary(x => x.CategoryId, x => x.DepartmentId.ToString() ?? "N/A");
        }),
    };
        if (branchId > 0)
        {
            tasks.Add(
                Task.Run(() =>
                {
                    using var scope = service.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
                    dbProductDetails.AddRange(from a in dbContext.ProductDetails
                                              join b in dbContext.Products on a.ProductId equals b.ProductId
                                              join c in dbContext.ProductCategories on b.ProductCategoryId equals c.CategoryId
                                              join d in dbContext.ProductDetailBranchMappings on a.ProductDetailId equals d.ProductDetailId
                                              where d.BranchId == branchId
                                              select a);

                }));
        }
        else
        {
            tasks.Add(
                Task.Run(() =>
                {
                    using var scope = service.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<Db.PgDbContext>();
                    dbProductDetails.AddRange(from a in dbContext.ProductDetails
                                              join b in dbContext.Products on a.ProductId equals b.ProductId
                                              join c in dbContext.ProductCategories on b.ProductCategoryId equals c.CategoryId
                                              where c.CompanyId == companyId
                                              select a);

                }));
        }

        await Task.WhenAll(tasks);

        foreach (var item in GetCategories(companyId, dbSizes, dbFlavours, dbProducts, dbProductDetails, dbDepartments, dbDealItemDetails, dbDealDescription))
        {
            yield return item;
        }
    }

    private IEnumerable<Category> GetCategories(int companyId, List<Db.ProductSize> dbSizes, List<Db.Flavour> dbFlavours, List<Db.Product> dbProducts, List<Db.ProductDetail> dbProductDetails, Dictionary<int, string> dbDepartments, List<Db.DealItemDetail> dbDealItemDetails, List<Db.DealDescription> dbDealDescription)
    {
        foreach (var dbCategory in (from x in dbContext.ProductCategories
                                    where x.CompanyId == companyId
                                    select x).ToList())
        {
            var category = new Category
            {
                Id = dbCategory.CategoryId.ToString(),
                Name = dbCategory.CategoryName ?? "N/A",
                Image = dbCategory.CategoryImage ?? "N/A",
                Items = [],
            };
            foreach (var dbProduct in dbProducts.Where(x => x.ProductCategoryId == dbCategory.CategoryId))
            {
                var item = new MenuItem
                {
                    Id = dbProduct.ProductId,
                    CategoryId = dbProduct.ProductCategoryId.ToString() ?? "0",
                    Name = dbProduct.ProductName ?? "N/A",
                    Image = dbProduct.ProductImage ?? "N/A",
                    DepartmentName = dbDepartments[dbProduct.ProductCategoryId ?? 0] ?? "N/A",
                };
                foreach (var dbProductDetail in dbProductDetails.Where(x => x.ProductId == dbProduct.ProductId))
                {
                    var variation = new ItemVariation
                    {
                        Id = dbProductDetail.ProductDetailId,
                        Size = (from x in dbSizes
                                where x.SizeId == dbProductDetail.SizeId
                                select new ItemSize
                                {
                                    Id = x.SizeId,
                                    Name = x.SizeName ?? "N/A",
                                }).First(),
                        Flavour = (from x in dbFlavours
                                   where x.FlavourId == dbProductDetail.FlavourId
                                   select new ItemFlavour
                                   {
                                       Id = x.FlavourId,
                                       Name = x.FlavourName ?? "N/A",
                                   }).First(),
                        Price = dbProductDetail.Price,
                    };
                    foreach (var dbDealItem in dbDealItemDetails.Where(x => x.ProductDetailId == dbProductDetail.ProductDetailId))
                    {
                        var itemChoice = new ItemChoice
                        {
                            Id = dbDealItem.DealItemId,
                            Name = dbDealItem.DealOptionName ?? "N/A",
                            Quantity = dbDealItem.Quantity ?? 0,
                            MaxChoice = dbDealItem.MaxQuantity ?? 0,
                        };
                        foreach (var dbDescription in dbDealDescription.Where(x => x.DealItemId == dbDealItem.DealItemId))
                        {
                            var list = (from x in dbProductDetails
                                        join y in dbProducts on x.ProductId equals y.ProductId
                                        where x.ProductDetailId == dbDescription.ProductDetailId
                                        select y).ToList();
                            var itemOption = new ItemOption
                            {
                                Id = dbDescription.ProductDetailId ?? 0,
                                Price = dbDescription.Price ?? 0.0,
                                Name = list.First().ProductName ?? string.Empty,
                            };
                            itemChoice.ItemOptions.Add(itemOption);
                        }
                        variation.ItemChoices.Add(itemChoice);
                    }
                    item.Variations.Add(variation);
                }
                category.Items.Add(item);
            }
            yield return category;
        }
    }
}