using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using System.Text.Json.Nodes;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Db = PointofSaleModels.PGDatabaseModels;

namespace DataService;

internal class Implementation
{
    private static Db.PgDbContext GetDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<Db.PgDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new Db.PgDbContext(options);
    }

    internal JsonObject GetDataOne(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var orderModes = new JsonObject();
        var delivery = new JsonObject();
        var pickup = new JsonObject();
        var cities = dbContext.Cities
            .Join(dbContext.Areas, c => c.CityId, a => a.CityId, (c, a) => c)
            .AsEnumerable()
            .DistinctBy(x => x.CityName)
            .ToList();
        var areas = dbContext.Areas
            .Join(dbContext.BranchDetails, a => a.AreaId, bd => bd.AreaId, (a, bd) => a)
            .ToList();
        var branches = dbContext.BranchMasters.ToList();
        var branchDetails = dbContext.BranchDetails.ToList();
        foreach (var item in cities)
        {
            var areasJsonArray = new JsonArray();
            foreach (var item1 in areas
                .Where(x => x.CityId == item.CityId)
                .Select(x => new JsonObject()
                {
                    ["AreaId"] = x.AreaId,
                    ["AreaName"] = x.AreaName
                }))
            {
                var areaId = item1["AreaId"]?.GetValue<int>();
                var branchDetail = branchDetails.FirstOrDefault(x => x.AreaId == areaId);
                if (branchDetail != null)
                {
                    item1["BranchId"] = branchDetail.BranchId;
                }

                areasJsonArray.Add(item1);
            }
            var cityObj = new JsonObject
            {
                ["CityName"] = item.CityName,
                ["Areas"] = areasJsonArray
            };
            delivery[item.CityId.ToString()] = cityObj;

            var branchesJsonArray = new JsonArray();
            foreach (var item2 in branches
                .Where(x => x.CityId == item.CityId)
                .Select(x => new JsonObject()
                {
                    ["BranchId"] = x.BranchId,
                    ["BranchName"] = x.BranchName,
                    ["BranchAddress"] = x.BranchAddress,
                    ["BranchPhoneNumber"] = x.BranchPhoneNumber,
                    ["BusinessStartTime"] = x.BusinessDayStartTime.ToString(),
                    ["BusinessEndTime"] = x.BusinessDayEndTime.ToString(),
                    ["IsBranchOpen"] = x.BusinessDayStartTime.HasValue && x.BusinessDayEndTime.HasValue
                        ? DateTime.UtcNow.TimeOfDay >= x.BusinessDayStartTime.Value.ToTimeSpan() && DateTime.UtcNow.TimeOfDay <= x.BusinessDayEndTime.Value.ToTimeSpan()
                        : false
                }))
            {
                var branchId = item2["BranchId"]?.GetValue<int>();
                var branchDetail = branchDetails.FirstOrDefault(bd => bd.BranchId == branchId);
                if (branchDetail != null)
                {
                    item2["DeliveryCharges"] = branchDetail.DeliveryCharges ?? 0.00;
                    item2["DeliveryChargesWaiveOffLimit"] = branchDetail.DeliveryChargesWaiveOffLimit ?? 0.00;
                    item2["DeliveryTime"] = branchDetail.DeliveryTime ?? 0;
                    item2["MinimumOrder"] = branchDetail.MinimumOrder ?? 0.00;
                }
                branchesJsonArray.Add(item2);
            }

            var cityObj2 = new JsonObject
            {
                ["CityName"] = item.CityName,
                ["Branches"] = branchesJsonArray
            };
            pickup[item.CityId.ToString()] = cityObj2;
        }
        orderModes["Delivery"] = delivery;
        orderModes["Pickup"] = pickup;
        return orderModes;
    }

    internal async IAsyncEnumerable<Category> GetMenuAsync(string connectionString, int branchId)
    {
        var bid = branchId;
        using var dbContext = GetDbContext(connectionString);
        if (bid == 0)
        {
            bid = dbContext.BranchMasters.First(x => x.IsActive ?? false).BranchId;
        }

        var dbProductDetailBranchMapping = await dbContext.ProductDetailBranchMappings
            .Where(x => x.BranchId == bid)
            .Select(x => x.ProductDetailId ?? 0)
            .ToListAsync();

        // Run queries sequentially to avoid concurrent DbConnection usage which can cause Npgsql EndOfStream errors
        var dbSizes = await dbContext.ProductSizes.ToListAsync();
        var dbFlavours = await dbContext.Flavours.ToListAsync();

        var dbProducts = await (from a in dbContext.Products
                                join b in dbContext.ProductDetails on a.ProductId equals b.ProductId
                                where dbProductDetailBranchMapping.Contains(b.ProductDetailId)
                                select a).Distinct().ToListAsync();

        var dbDealItemDetails = await (from a in dbContext.DealItemDetails
                                       join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                                       join c in dbContext.Products on b.ProductId equals c.ProductId
                                       select a).ToListAsync();

        var dbDealDescription = await (from a in dbContext.DealDescriptions
                                       join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                       join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                       join d in dbContext.Products on c.ProductId equals d.ProductId
                                       select a).ToListAsync();

        var dbDealDescriptionProducts = await (from a in dbContext.DealDescriptions
                                               join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                               join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                               join d in dbContext.Products on c.ProductId equals d.ProductId
                                               join e in dbContext.ProductDetails on a.ProductDetailId equals e.ProductDetailId
                                               join f in dbContext.Products on e.ProductId equals f.ProductId
                                               select f).Distinct().ToListAsync();

        var dbDepartments = await dbContext.ProductCategories
            .Select(a => new { a.CategoryId, a.DepartmentId })
            .ToDictionaryAsync(x => x.CategoryId, x => x.DepartmentId.ToString() ?? "N/A");

        var dbProductDetails = await (from a in dbContext.ProductDetails
                                       join b in dbContext.Products on a.ProductId equals b.ProductId
                                       where dbProductDetailBranchMapping.Contains(a.ProductDetailId)
                                       select a).ToListAsync();

        var dbItemDiscounts = await (from a in dbContext.Discounts
                                     join b in dbContext.DiscountProductDetailMappings on a.DiscountId equals b.DiscountId
                                     join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                     where dbProductDetailBranchMapping.Contains(c.ProductDetailId)
                                     select a).ToListAsync();

        var dbItemDiscountsMapping = await (from a in dbContext.DiscountProductDetailMappings
                                            join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                                            where dbProductDetailBranchMapping.Contains(b.ProductDetailId)
                                            select a).ToListAsync();

        var package = new DbMenuData(dbSizes, dbFlavours, dbProducts, dbProductDetails, dbDepartments, dbDealItemDetails, dbDealDescription, dbItemDiscounts, dbItemDiscountsMapping);
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
                Items = new List<MenuItem>(),
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
                    var itemDiscount = dbMenuData.ItemDiscounts.Join(
                                        dbMenuData.DiscountMappings,
                                        a => a.DiscountId,
                                        b => b.DiscountId,
                                        (a, b) => new { Discount = a, DiscountMapping = b })
                                    .Where(x => x.DiscountMapping.ProductDetailId == dbProductDetail.ProductDetailId)
                                    .Select(x => x.Discount)
                                    .FirstOrDefault();

                    if (itemDiscount != null)
                    {
                        item.Discount = new Discount
                        {
                            Id = itemDiscount.DiscountId,
                            Name = itemDiscount.DiscountName ?? string.Empty,
                            MaxCap = decimal.ToDouble(itemDiscount.DiscountCapEnd),
                            MinCap = decimal.ToDouble(itemDiscount.DiscountCapStart),
                            Type = itemDiscount.IsPercentage ? PointofSaleModels.Application.ValueType.Percentage : PointofSaleModels.Application.ValueType.Amount,
                            Value = itemDiscount.DiscountPercent
                        };
                    }

                    var variation = new ItemVariation
                    {
                        Id = dbProductDetail.ProductDetailId,
                        Size = (from x in dbMenuData.ProductSizes
                                where x.SizeId == dbProductDetail.SizeId
                                select new ItemSize
                                {
                                    Id = x.SizeId,
                                    Name = x.SizeName ?? "N/A",
                                }).First(),
                        Flavour = (from x in dbMenuData.Flavours
                                   where x.FlavourId == dbProductDetail.FlavourId
                                   select new ItemFlavour
                                   {
                                       Id = x.FlavourId,
                                       Name = x.FlavourName ?? "N/A",
                                   }).First(),
                        Price = dbProductDetail.Price,
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
                                Name = list.First().ProductName ?? string.Empty,
                            };
                            itemChoice.ItemOptions.Add(itemOption);
                        }
                        variation.ItemChoices.Add(itemChoice);
                    }
                    if (item.Price == 0.0 || item.Price > variation.Price)
                    {
                        item.Price = variation.Price;
                    }
                    item.Variations.Add(variation);
                }
                category.Items.Add(item);
            }
            yield return category;
        }
    }

    private record DbMenuData(List<Db.ProductSize> ProductSizes, List<Db.Flavour> Flavours, List<Db.Product> Products, List<Db.ProductDetail> ProductDetails, Dictionary<int, string> Departments, List<Db.DealItemDetail> DealItemDetails, List<Db.DealDescription> DealDescriptions, List<Db.Discount> ItemDiscounts, List<Db.DiscountProductDetailMapping> DiscountMappings);
}