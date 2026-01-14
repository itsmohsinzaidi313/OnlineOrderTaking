using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PointofSaleModels.Application;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Db = PointofSaleModels.PGDatabaseModels;

namespace DataService;

internal class Implementation()
{
    internal async IAsyncEnumerable<object> GetBranchesAsync(string connectionString)
    {
        var dbContext = getDbContext(connectionString);
        var branches = from a in dbContext.BranchMasters
                       join b in dbContext.BranchDetails on a.BranchId equals b.BranchId
                       join c in dbContext.Areas on b.AreaId equals c.AreaId
                       group c by new
                       {
                           a.BranchId,
                           a.BranchName,
                           a.BranchAddress,
                           a.BusinessDayStartTime,
                           a.BusinessDayEndTime,
                           a.BranchPhoneNumber
                       } into g
                       select new
                       {
                           Id = g.Key.BranchId,
                           Name = g.Key.BranchName ?? "N/A",
                           Address = g.Key.BranchAddress ?? "N/A",
                           StartTime = g.Key.BusinessDayStartTime,
                           EndTime = g.Key.BusinessDayEndTime,
                           Contact = g.Key.BranchPhoneNumber ?? "N/A"
                       };
        foreach (var branch in branches)
        {
            yield return branch;
        }
    }

    internal async IAsyncEnumerable<object> GetAreasAsync(string connectionString)
    {
        var dbContext = getDbContext(connectionString);
        var areas = from a in dbContext.Areas
                    join b in dbContext.BranchDetails on a.AreaId equals b.AreaId
                    join c in dbContext.BranchMasters on b.BranchId equals c.BranchId
                    group c by new
                    {
                        a.AreaId,
                        a.AreaName
                    } into g
                    select new
                    {
                        Id = g.Key.AreaId,
                        Name = g.Key.AreaName ?? "N/A",
                        Branches = g.Select(x => new
                        {
                            Id = x.BranchId,
                            Name = x.BranchName ?? "N/A",
                            Address = x.BranchAddress ?? "N/A",
                            StartTime = x.BusinessDayStartTime,
                            EndTime = x.BusinessDayEndTime,
                            Contact = x.BranchPhoneNumber ?? "N/A"
                        })
                    };
        foreach (var area in areas)
        {
            yield return area;
        }
    }

    Db.PgDbContext getDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<Db.PgDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new Db.PgDbContext(options);
    }
    internal async IAsyncEnumerable<Category> GetMenuAsync(string connectionString, int branchId)
    {
        var dbSizes = new List<Db.ProductSize>();
        var dbFlavours = new List<Db.Flavour>();
        var dbProducts = new List<Db.Product>();
        var dbProductDetails = new List<Db.ProductDetail>();
        var dbDealItemDetails = new List<Db.DealItemDetail>();
        var dbDealDescription = new List<Db.DealDescription>();
        var dbDealDescriptionProducts = new List<Db.Product>();
        var dbDepartments = new Dictionary<int, string>();
        var dbItemDiscounts = new List<Db.Discount>();
        var dbBranchDiscounts = new List<Db.Discount>();

        var tasks = new List<Task>
        {
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbSizes.AddRange(from a in dbContext.ProductSizes
                                 select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbFlavours.AddRange(from a in dbContext.Flavours
                                    select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbProducts.AddRange(from a in dbContext.Products
                                    join b in dbContext.ProductCategories on a.ProductCategoryId equals b.CategoryId
                                    join c in dbContext.ProductDetails on a.ProductId equals c.ProductId
                                    join d in dbContext.ProductDetailBranchMappings on c.ProductDetailId equals d.ProductDetailId
                                    where d.BranchId == branchId
                                    select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbDealItemDetails.AddRange(from a in dbContext.DealItemDetails
                                           join c in dbContext.ProductDetails on a.ProductDetailId equals c.ProductDetailId
                                           join d in dbContext.Products on c.ProductId equals d.ProductId
                                           join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                           join f in dbContext.ProductDetailBranchMappings on c.ProductDetailId equals f.ProductDetailId
                                           where f.BranchId == branchId
                                           select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbDealDescription.AddRange(from a in dbContext.DealDescriptions
                                           join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                           join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                           join d in dbContext.Products on c.ProductId equals d.ProductId
                                           join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                           join f in dbContext.ProductDetailBranchMappings on c.ProductDetailId equals f.ProductDetailId
                                           where f.BranchId == branchId
                                           select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbDealDescriptionProducts.AddRange(from a in dbContext.DealDescriptions
                                                   join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                                   join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                                   join d in dbContext.Products on c.ProductId equals d.ProductId
                                                   join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                                   join f in dbContext.ProductDetails on a.ProductDetailId equals f.ProductDetailId
                                                   join g in dbContext.Products on f.ProductId equals g.ProductId
                                                   join h in dbContext.ProductDetailBranchMappings on b.ProductDetailId equals h.ProductDetailId
                                                   where h.BranchId == branchId
                                                   select g);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbDepartments = (from a in dbContext.ProductCategories
                                select new { a.CategoryId, a.DepartmentId }).ToDictionary(x => x.CategoryId, x => x.DepartmentId.ToString() ?? "N/A");
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbProductDetails.AddRange(from a in dbContext.ProductDetails
                                            join b in dbContext.Products on a.ProductId equals b.ProductId
                                            join c in dbContext.ProductCategories on b.ProductCategoryId equals c.CategoryId
                                            join d in dbContext.ProductDetailBranchMappings on a.ProductDetailId equals d.ProductDetailId
                                            where d.BranchId == branchId
                                            select a);

            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbItemDiscounts.AddRange(from a in dbContext.Discounts
                                            join b in dbContext.DiscountProductDetailMappings on a.DiscountId equals b.DiscountId
                                            join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                            join d in dbContext.ProductDetailBranchMappings on c.ProductDetailId equals d.ProductDetailId
                                            where d.BranchId == branchId
                                            select a);
            }),
            Task.Run(() =>
            {
                using var dbContext = getDbContext(connectionString);
                dbBranchDiscounts.AddRange(from a in dbContext.BranchMasters
                                           join b in dbContext.DiscountBranchMappings on a.BranchId equals b.BranchId
                                           join c in dbContext.Discounts on b.DiscountId equals c.DiscountId
                                           where a.BranchId == branchId
                                           select c);
            })
        };

        await Task.WhenAll(tasks);

        foreach (var item in GetCategories(connectionString, dbSizes, dbFlavours, dbProducts, dbProductDetails, dbDepartments, dbDealItemDetails, dbDealDescription))
        {
            yield return item;
        }
    }

    internal JsonObject GetDataOne(string connectionString)
    {
        using var dbContext = getDbContext(connectionString);
        var orderModes = new JsonObject();
        var delivery = new JsonObject();
        var pickup = new JsonObject();
        var cities = dbContext.Cities.ToList();
        var areas = dbContext.Areas.ToList();
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
                    ["BusinessEndTime"] = x.BusinessDayEndTime.ToString()
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

    private IEnumerable<Category> GetCategories(string connectionString, List<Db.ProductSize> dbSizes, List<Db.Flavour> dbFlavours, List<Db.Product> dbProducts, List<Db.ProductDetail> dbProductDetails, Dictionary<int, string> dbDepartments, List<Db.DealItemDetail> dbDealItemDetails, List<Db.DealDescription> dbDealDescription)
    {
        using var dbContext = getDbContext(connectionString);
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
            foreach (var dbProduct in dbProducts.Where(x => x.ProductCategoryId == dbCategory.CategoryId))
            {
                var item = new MenuItem
                {
                    Id = dbProduct.ProductId,
                    CategoryId = dbProduct.ProductCategoryId.ToString() ?? "0",
                    Name = dbProduct.ProductName ?? "N/A",
                    Image = dbProduct.ProductImage ?? "N/A",
                    DepartmentName = dbDepartments[dbProduct.ProductCategoryId ?? 0] ?? "N/A",
                    Description = dbProduct.ProductDescription ?? "N/A",
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
}