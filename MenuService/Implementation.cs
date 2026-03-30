using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace MenuService;

internal class Implementation()
{
    private static Db.PgDbContext GetDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<Db.PgDbContext>()
            .UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            })
            .Options;
        return new Db.PgDbContext(options);
    }

    private async Task<DbMenuData> GetDbMenuDataAsync(string connectionString, int branchId)
    {
        var bid = branchId;
        if (bid == 0)
        {
            using var dbContext = GetDbContext(connectionString);
            var activeBranch = dbContext.BranchMasters.FirstOrDefault(x => x.IsActive);
            if (activeBranch != null)
                bid = activeBranch.BranchId;
            else
                bid = 0;
        }
        var dbSizes = new List<Db.ProductSize>();
        var dbFlavours = new List<Db.Flavour>();
        var dbProducts = new List<Db.Product>();
        var dbProductDetails = new List<Db.ProductDetail>();
        var dbDealItemDetails = new List<Db.DealItemDetail>();
        var dbDealDescription = new List<Db.DealDescription>();
        var dbDealDescriptionProducts = new List<Db.Product>();
        var dbDepartments = new Dictionary<int, string>();
        var dbItemDiscounts = new List<Db.Discount>();
        var dbItemDiscountsMapping = new List<Db.DiscountProductDetailMapping>();
        var dbOrderModeDiscountMapping = new List<Db.DiscountOrderModeMapping>();
        var dbOrderModes = new List<Db.SetupMasterDetail>();
        var dbProductDetailBranchMapping = new List<int>();
        {
            using var dbContext = GetDbContext(connectionString);
            dbProductDetailBranchMapping.AddRange([.. dbContext.ProductDetailBranchMappings.Where(x => x.BranchId == bid).Select(x => x.ProductDetailId ?? 0)]);
        }

        var tasks = new List<Task>
        {
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbSizes.AddRange([.. from a in dbContext.ProductSizes
                                 select a]);
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbFlavours.AddRange([.. from a in dbContext.Flavours
                                    select a]);
            }),
            Task.Run(async () =>
            {
                using var dbContext = GetDbContext(connectionString);

                dbProducts.AddRange(await (from a in dbContext.Products
                                       join b in dbContext.ProductDetails on a.ProductId equals b.ProductId
                                       where dbProductDetailBranchMapping.Contains(b.ProductDetailId) && a.IsEnable && a.DisplayInWeb
                                       orderby a.SortOrder ascending
                                       select a).Distinct().ToListAsync());
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbDealItemDetails.AddRange([.. from a in dbContext.DealItemDetails
                                           select a]);
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbDealDescription.AddRange([.. from x in dbContext.DealDescriptions select x]);
            }),
            Task.Run(async () =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbDealDescriptionProducts.AddRange([.. (from a in dbContext.Products
                                                   join b in dbContext.ProductDetails on a.ProductId equals b.ProductId
                                                   join c in dbContext.DealDescriptions on b.ProductDetailId equals c.ProductDetailId
                                                   select a).Distinct()]);
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbDepartments = (from a in dbContext.ProductCategories
                                select new { a.CategoryId, a.DepartmentId }).ToDictionary(x => x.CategoryId, x => x.DepartmentId.ToString() ?? "N/A");
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbProductDetails.AddRange([.. from a in dbContext.ProductDetails
                                            where dbProductDetailBranchMapping.Contains(a.ProductDetailId)
                                            select a]);

            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbItemDiscounts.AddRange([.. (from a in dbContext.Discounts
                                            join b in dbContext.DiscountProductDetailMappings on a.DiscountId equals b.DiscountId
                                            join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                            join d in dbContext.DiscountBranchMappings on a.DiscountId equals d.DiscountId
                                            where dbProductDetailBranchMapping.Contains(c.ProductDetailId) && d.BranchId == bid && (a.IsActiveInWeb ?? false) == true
                                            select a).Distinct()]);
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbItemDiscountsMapping.AddRange([.. (from a in dbContext.DiscountProductDetailMappings
                                                join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                                                where dbProductDetailBranchMapping.Contains(b.ProductDetailId)
                                                select a).Distinct()]);
            }),
            Task.Run(() =>             {
                using var dbContext = GetDbContext(connectionString);
                dbOrderModeDiscountMapping.AddRange([.. (from a in dbContext.DiscountOrderModeMappings
                                                        join b in dbContext.Discounts on a.DiscountId equals b.DiscountId
                                                        join c in dbContext.DiscountBranchMappings on b.DiscountId equals c.DiscountId
                                                        where  c.BranchId == bid
                                                        select a).Distinct()]);
             }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbOrderModes.AddRange([.. from a in dbContext.SetupMasterDetails
                                        where a.SetupMasterId == 4
                                        select a]);
             }),
        };

        await Task.WhenAll(tasks);
        return new DbMenuData(dbSizes, dbFlavours, dbProducts, dbProductDetails, dbDepartments, dbDealItemDetails, dbDealDescription, dbItemDiscounts, dbItemDiscountsMapping, dbOrderModeDiscountMapping, dbOrderModes);
    }

    internal async IAsyncEnumerable<Category> GetMenuAsync(string connectionString, int branchId)
    {

        var package = await GetDbMenuDataAsync(connectionString, branchId);
        foreach (var item in GetCategories(connectionString, package))
        {
            yield return item;
        }
    }

    private static IEnumerable<Category> GetCategories(string connectionString, DbMenuData dbMenuData)
    {
        using var dbContext = GetDbContext(connectionString);
        foreach (var dbCategory in (from x in dbContext.ProductCategories
                                    select x).ToList())
        {
            var category = new Category
            {
                Id = dbCategory.CategoryId.ToString(),
                Name = dbCategory.CategoryName ?? "N/A",
                Image = dbCategory.CategoryImage ?? "N/A",
                Icon = dbCategory.CategoryIcon ?? "N/A",
                Items = [],
            };
            foreach (var dbProduct in dbMenuData.Products.Where(x => x.ProductCategoryId == dbCategory.CategoryId))
            {
                var item = new MenuItem
                {
                    Id = dbProduct.ProductId,
                    CategoryId = dbProduct.ProductCategoryId.ToString() ?? "0",
                    Name = dbProduct.ProductName ?? "N/A",
                    Image = dbProduct.ProductImage ?? "N/A",
                    DepartmentName = dbMenuData.Departments[dbProduct.ProductCategoryId ?? 0] ?? "N/A",
                    Description = dbProduct.ProductDescription ?? "N/A",
                };
                foreach (var dbProductDetail in dbMenuData.ProductDetails.Where(x => x.ProductId == dbProduct.ProductId))
                {
                    //var orderMode = dbMenuData.OrderModes.Join(dbMenuData.OrderModeDiscountMappings, a => a.SetupDetailId, b => b.OrderModeId, (a, b) => new { OrderMode = a.Flex1, b.DiscountId })
                                        //.ToDictionary(x => x.DiscountId, x => x.OrderMode);
                    var itemDiscount = dbMenuData.ItemDiscounts.Join(
                                        dbMenuData.DiscountMappings,
                                        a => a.DiscountId,
                                        b => b.DiscountId,
                                        (a, b) => new { Discount = a, DiscountMapping = b })
                                    .Where(x => x.DiscountMapping.ProductDetailId == dbProductDetail.ProductDetailId)
                                    .Select(x => new Discount
                                    {
                                        Id = x.Discount.DiscountId,
                                        Name = x.Discount.DiscountName ?? string.Empty,
                                        MaxCap = decimal.ToDouble(x.Discount.DiscountCapEnd),
                                        MinCap = decimal.ToDouble(x.Discount.DiscountCapStart),
                                        Type = x.Discount.IsPercentage ? PointofSaleModels.Application.ValueType.Percentage.ToString() : PointofSaleModels.Application.ValueType.Amount.ToString(),
                                        Value = x.Discount.DiscountPercent,
                                        //OrderType = orderMode[x.Discount.DiscountId]
                                    })
                                    .FirstOrDefault();

                    var sizeItem = (from x in dbMenuData.ProductSizes
                                    where x.SizeId == dbProductDetail.SizeId
                                    select new ItemSize
                                    {
                                        Id = x.SizeId,
                                        Name = x.SizeName ?? "N/A",
                                    }).FirstOrDefault() ?? new ItemSize { Id = dbProductDetail.SizeId, Name = "N/A" };

                    var flavourItem = (from x in dbMenuData.Flavours
                                       where x.FlavourId == dbProductDetail.FlavourId
                                       select new ItemFlavour
                                       {
                                           Id = x.FlavourId,
                                           Name = x.FlavourName ?? "N/A",
                                       }).FirstOrDefault() ?? new ItemFlavour { Id = dbProductDetail.FlavourId ?? 0, Name = "N/A" };

                    var variation = new ItemVariation
                    {
                        Id = dbProductDetail.ProductDetailId,
                        Size = sizeItem,
                        Flavour = flavourItem,
                        Price = dbProductDetail.Price,
                        Discount = itemDiscount
                    };
                    foreach (var dbDealItem in dbMenuData.DealItemDetails.Where(x => x.ProductDetailId == dbProductDetail.ProductDetailId))
                    {
                        var itemChoice = new ItemChoice
                        {
                            Id = dbDealItem.DealItemId,
                            Name = dbDealItem.DealOptionName ?? "N/A",
                            Quantity = dbDealItem.Quantity ?? 0,
                            MaxChoice = dbDealItem.MaxQuantity ?? 0,
                        };
                        foreach (var dbDescription in dbMenuData.DealDescriptions.Where(x => x.DealItemId == dbDealItem.DealItemId))
                        {
                            var list = (from x in dbMenuData.ProductDetails
                                        join y in dbMenuData.Products on x.ProductId equals y.ProductId
                                        where x.ProductDetailId == dbDescription.ProductDetailId
                                        select y).ToList();
                            var itemOption = new ItemOption
                            {
                                Id = dbDescription.ProductDetailId ?? 0,
                                Price = dbDescription.Price ?? 0.0,
                                Name = list.FirstOrDefault()?.ProductName ?? string.Empty,
                            };
                            itemChoice.ItemOptions.Add(itemOption);
                        }
                        variation.ItemChoices.Add(itemChoice);
                    }
                    if (item.Price == 0.0 || item.Price > variation.Price)
                    {
                        item.Price = variation.Price;
                        item.Discount = variation.Discount;
                    }
                    item.Variations.Add(variation);
                }
                category.Items.Add(item);
            }
            if (category.Items.Count >= 1)
            {
                yield return category;
            }
        }
    }

    private record DbMenuData(List<Db.ProductSize> ProductSizes, List<Db.Flavour> Flavours, List<Db.Product> Products, List<Db.ProductDetail> ProductDetails, Dictionary<int, string> Departments, List<Db.DealItemDetail> DealItemDetails, List<Db.DealDescription> DealDescriptions, List<Db.Discount> ItemDiscounts, List<Db.DiscountProductDetailMapping> DiscountMappings, List<Db.DiscountOrderModeMapping> OrderModeDiscountMappings, List<Db.SetupMasterDetail> OrderModes);
}