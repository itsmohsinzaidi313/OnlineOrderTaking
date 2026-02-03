using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace DataService;

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

    internal async Task<JsonObject> GetDataOneAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var orderModes = new JsonObject();
        var delivery = new JsonObject();
        var pickup = new JsonObject();
        var cities = (await (from x in dbContext.Cities join y in dbContext.Areas on x.CityId equals y.CityId select x).ToListAsync()).DistinctBy(x => x.CityName);
        var areas = await (from x in dbContext.Areas join y in dbContext.BranchDetails on x.AreaId equals y.AreaId select x).ToListAsync();
        var branches = await dbContext.BranchMasters.ToListAsync();
        var branchDetails = await dbContext.BranchDetails.ToListAsync();
        var setupMasterId = await dbContext.SetupMasters.Where(x => x.SetupMasterName == "Day").Select(x => x.SetupMasterId).FirstOrDefaultAsync();
        var days = dbContext.SetupMasterDetails
                    .Where(x => x.SetupMasterId == setupMasterId)
                    .Distinct()
                    .ToDictionary(x => x.SetupDetailId, x => x.SetupDetailName);

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
                }))
            {
                var branchId = item2["BranchId"]?.GetValue<int>();
                var businessDaysMapping = await dbContext.BranchDayMappings.Where(x => x.BranchId == branchId).ToListAsync();
                var todayDow = DateTime.Today.DayOfWeek.ToString();
                var isBranchOpen = false;
                foreach (var dayMapping in businessDaysMapping)
                {
                    var value = days[dayMapping.DayId]?.ToString() ?? string.Empty;
                    if (value == todayDow)
                    {
                        var startTime = dayMapping.StartTime;
                        var endTime = dayMapping.EndTime;
                        var timeOfDay = DateTime.Now.TimeOfDay;
                        if (startTime > endTime)
                        {
                            var maybeOpen = startTime > timeOfDay;
                            if (maybeOpen)
                            {
                                isBranchOpen = true;
                            }
                            else
                            {
                                isBranchOpen = timeOfDay > endTime;
                            }
                        }
                        else if (startTime < endTime)
                        {
                            if (timeOfDay >= startTime && timeOfDay <= endTime)
                            {
                                isBranchOpen = true;
                            }
                        }
                        item2["IsBranchOpen"] = isBranchOpen;
                        break;
                    }
                }
                item2["IsBranchOpen"] = isBranchOpen;

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
        var themeData = new JsonObject
        {
            ["Colors"] = await GetThemeDataAsync(connectionString),
            ["Settings"] = await GetSettingsDataAsync(connectionString)
        };
        orderModes["Theme"] = themeData;
        return orderModes;
    }

    private static async Task<JsonObject> GetThemeDataAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var keys = new[]
        {
            "TOP_BAR_BG_COLOR",
            "TOP_BAR_FORE_COLOR",
            "CATEGORY_BAR_BG_COLOR",
            "CATEGORY_BAR_FORE_COLOR",
            "CATEGORY_HOVER_COLOR",
            "CATEGORY_ACTIVE_COLOR",
            "PRODUCT_BG_COLOR",
            "PRODUCT_NAME_FORE_COLOR",
            "PRODUCT_DESC_FORE_COLOR",
            "PRODUCT_HOVER_COLOR",
            "PRODUCT_PRICE_BG_COLOR",
            "PRODUCT_PRICE_FORE_COLOR",
            "PRODUCT_ADD_BTN_BG_COLOR",
            "FOOTER_BG_COLOR",
            "FOOTER_FORE_COLOR",
            "VIEW_CART_BG_COLOR",
            "VIEW_CART_FORE_COLOR",
            "PRODUCT_POPUP_BG_COLOR",
            "PRODUCT_POPUP_HEADER_BG_COLOR",
            "PRODUCT_POPUP_HEADER_FORE_COLOR",
            "PRODUCT_POPUP_DESC_FORE_COLOR",
            "PRODUCT_POPUP_PRICE_FORE_COLOR",
            "PRODUCT_POPUP_ADD_TO_CART_FORE_COLOR",
            "PRODUCT_POPUP_ADD_TO_CART_BG_COLOR",
            "PRODUCT_POPUP_PLUS_MINUS_BG_COLOR",
            "PRODUCT_POPUP_QTY_FORE_COLOR",
            "DEAL_POPUP_OPTION_NAME_FORE_COLOR",
            "DEAL_POPUP_PRODUCT_NAME_FORE_COLOR",
            "PRIMARY_COLOR",
            "SECONDARY_COLOR",
            "WEB_BG_COLOR"
        };
        var settings = await dbContext.SetupMasterDetails
            .Where(x => keys.Contains(x.Flex1))
            .ToDictionaryAsync(x => x.Flex1!, x => x.SetupDetailId);

        var setupDetailIds = settings.Values.ToList();

        var settingsDetail = await dbContext.SetupCompanySettings
            .Where(x => setupDetailIds.Contains(x.SetupDetailId ?? 0))
            .ToDictionaryAsync(x => x.SetupDetailId ?? 0, x => x.SettingValue ?? string.Empty);

        var colorData = new JsonObject();


        foreach (var key in keys)
        {
            string resolved = string.Empty;
            if (settings.TryGetValue(key, out var detailId) && settingsDetail.TryGetValue(detailId, out var value))
            {
                resolved = value ?? string.Empty;
            }

            colorData[key] = JsonValue.Create(resolved);
        }

        return colorData;
    }

    private static async Task<JsonObject> GetSettingsDataAsync(string connectionString)
    {
        var settingsData = new JsonObject();
        using var dbContext = GetDbContext(connectionString);
        var keys = new[]
        {
            "UPLOAD_LOGO",
            "UPLOAD_SPLASH_BANNER",
            "UPLOAD_BACKGROUND",
        };

        var settings = await dbContext.SetupMasterDetails
            .Where(x => keys.Contains(x.Flex1))
            .ToDictionaryAsync(x => x.Flex1, x => x.SetupDetailId);
        var setupDetailIds = settings.Values.ToList();
        var settingsDetail = await dbContext.SetupCompanySettings.Where(x => setupDetailIds.Contains(x.SetupDetailId ?? 0)).ToDictionaryAsync(x => x.SettingId, x => x.SettingValue);

        var uploadLogoId = settings.GetValueOrDefault("UPLOAD_LOGO", 0);
        var uploadSplashBannerId = settings.GetValueOrDefault("UPLOAD_SPLASH_BANNER", 0);
        var uploadBackgroundId = settings.GetValueOrDefault("UPLOAD_BACKGROUND", 0);

        var restaurantLogo = settingsDetail.GetValueOrDefault(uploadLogoId, string.Empty);
        var splashBanner = settingsDetail.GetValueOrDefault(uploadSplashBannerId, string.Empty);
        var websiteBackgroundImage = settingsDetail.GetValueOrDefault(uploadBackgroundId, string.Empty);

        settingsData["RESTAURANT_LOGO"] = restaurantLogo;
        settingsData["SPLASH_BANNER"] = splashBanner;
        settingsData["WEBSITE_BACKGROUND_IMAGE"] = websiteBackgroundImage;

        var s = await dbContext.SetupMasterDetails.Where(x => x.Flex1 == "UPLOAD_BANNER").Select(x => x.SetupDetailId).FirstOrDefaultAsync();
        var s2 = await dbContext.SetupCompanySettings.Where(x => x.SetupDetailId == s).ToListAsync();
        var array = new JsonArray();
        foreach (var item in s2)
        {
            array.Add(item.SettingValue);
        }
        settingsData["BANNER_IMAGES"] = array;

        settingsData["HEADER_LAYOUT_STYLE"] = "default";
        settingsData["FOOTER_LAYOUT_STYLE"] = "default";
        settingsData["CATEGORY_BAR_LAYOUT_STYLE"] = "default";
        settingsData["PRODUCT_CARD_LAYOUT_STYLE"] = "default";
        settingsData["SUBMIT_COMPLAINT_BUTTON"] = false;
        settingsData["MULTI_LANGUAGE"] = false;
        settingsData["USER_LOGIN_ICON"] = false;
        settingsData["HAMBURGER_MENU"] = false;
        settingsData["ABOUT_US"] = false;

        return settingsData;
    }

    private static async Task<DbMenuData> GetDbMenuDataAsync(string connectionString, int branchId)
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
        var dbProductDetailBranchMapping = new List<int>();
        {
            var dbContext = GetDbContext(connectionString);
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
                                       where dbProductDetailBranchMapping.Contains(b.ProductDetailId)
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
                                            where dbProductDetailBranchMapping.Contains(c.ProductDetailId)
                                            select a).Distinct()]);
            }),
            Task.Run(() =>
            {
                using var dbContext = GetDbContext(connectionString);
                dbItemDiscountsMapping.AddRange([.. (from a in dbContext.DiscountProductDetailMappings
                                                join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                                                where dbProductDetailBranchMapping.Contains(b.ProductDetailId)
                                                select a).Distinct()]);
            })
        };

        await Task.WhenAll(tasks);
        return new DbMenuData(dbSizes, dbFlavours, dbProducts, dbProductDetails, dbDepartments, dbDealItemDetails, dbDealDescription, dbItemDiscounts, dbItemDiscountsMapping);
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
                                        Value = x.Discount.DiscountPercent
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

    internal async IAsyncEnumerable<CustomerOrder> GetOrdersAsync(string connectionString, int branchId)
    {
        var dbContext = GetDbContext(connectionString);
        var menuItemIds = await dbContext.ProductDetails
            .ToDictionaryAsync(x => x.ProductDetailId, y => y.ProductId);

        var productDicts = await dbContext.Products
            .ToDictionaryAsync(x => x.ProductId, x => x.ProductName ?? "N/A");

        var categoryIds = await dbContext.Products
            .Join(dbContext.ProductCategories, a => a.ProductCategoryId, b => b.CategoryId,
                (a, b) => new { a.ProductId, b.CategoryId })
            .ToDictionaryAsync(x => x.ProductId, y => y.CategoryId);
        var dealItemDetails = await dbContext.DealItemDetails.ToListAsync();
        var dealDescriptions = await dbContext.DealDescriptions.ToListAsync();

        foreach (var dbOrder in await dbContext.OrderMasters.Where(x => x.BranchId == branchId && x.OrderMasterId >= 130560).ToListAsync())
        {
            var order = new CustomerOrder
            {
                OrderNumber = dbOrder.OrderNumber ?? "N/A",
                Items = [],
            };

            var phoneId = dbOrder.PhoneId;
            var customerPhone = await dbContext.CustomerPhones.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();
            if (customerPhone != null)
            {
                var customer = await dbContext.Customers.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();
                var addressDetails = await dbContext.CustomerAddressDetails.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();

                var customerDetail = new CustomerDetail
                {
                    FullName = customer?.CustomerName ?? "N/A",
                    MobileNumber = customerPhone.PhoneNumber ?? "N/A",
                    DeliveryAddress = addressDetails?.CompleteAddress ?? "N/A",
                    NearestLandmark = addressDetails?.LandMark ?? "N/A",
                    DeliveryInstructions = addressDetails?.Remarks ?? "N/A"
                };
                order.CustomerDetails = customerDetail;
            }


            var orderDetails = await dbContext.OrderDetails.Where(x => x.OrderMasterId == dbOrder.OrderMasterId).ToListAsync();

            foreach (var dbOd in orderDetails)
            {
                if(dbOd.DealItemId != null)
                {
                    continue;
                }

                var productId = menuItemIds[dbOd.ProductDetailId];
                var productname = productDicts[productId];
                var categoryId = categoryIds[productId];
                var item = new MenuItem
                {
                    Id = productId,
                    Name = productname,
                    Quantity = dbOd.Quantity ?? 0,
                    CategoryId = categoryIds[productId].ToString(),
                };

                var addons = orderDetails.Where(x => x.RandomId == dbOd.RandomId).ToList();

                foreach (var addon in addons)
                {
                    var addonPdetId = addon.ProductDetailId;

                    var sizeItem = (from x in dbContext.ProductSizes
                                    join y in dbContext.ProductDetails on x.SizeId equals y.SizeId
                                    where y.ProductDetailId == addonPdetId
                                    select new ItemSize
                                    {
                                        Id = x.SizeId,
                                        Name = x.SizeName ?? "N/A",
                                    }).FirstOrDefault() ?? new ItemSize { Id = 0, Name = "N/A" };

                    var flavourItem = (from x in dbContext.Flavours
                                       join y in dbContext.ProductDetails on x.FlavourId equals y.FlavourId
                                       where y.ProductDetailId == addonPdetId
                                       select new ItemFlavour
                                       {
                                           Id = x.FlavourId,
                                           Name = x.FlavourName ?? "N/A",
                                       }).FirstOrDefault() ?? new ItemFlavour { Id = 0, Name = "N/A" };
                    var variation = new ItemVariation
                    {
                        Id = addonPdetId,
                        Size = sizeItem,
                        Flavour = flavourItem,
                        Price = dbOd.PriceWithoutGst ?? 0.0,
                    };


                    var dealItemId = addon.DealItemId;
                    if (dealItemId != null)
                    {
                        var dealItem = dealItemDetails.FirstOrDefault(x => x.DealItemId == dealItemId);
                        if (dealItem != null)
                        {
                            var itemChoice = new ItemChoice
                            {
                                Id = dealItem.DealItemId,
                                Name = dealItem.DealOptionName ?? "N/A",
                                Quantity = dealItem.Quantity ?? 0,
                                MaxChoice = dealItem.MaxQuantity ?? 0,
                            };
                            var descriptions = dealDescriptions.Where(x => x.DealItemId == dealItem.DealItemId).ToList();
                            foreach (var desc in descriptions)
                            {
                                var list = (from x in dbContext.ProductDetails
                                            join y in dbContext.Products on x.ProductId equals y.ProductId
                                            where x.ProductDetailId == desc.ProductDetailId
                                            select y).ToList();
                                var itemOption = new ItemOption
                                {
                                    Id = desc.ProductDetailId ?? 0,
                                    Price = desc.Price ?? 0.0,
                                    Name = list.FirstOrDefault()?.ProductName ?? string.Empty,
                                };
                                itemChoice.ItemOptions.Add(itemOption);
                            }
                            item.Price = variation.Price + itemChoice.ItemOptions.Sum(x => x.Price);
                            variation.ItemChoices.Add(itemChoice);
                        }
                    }
                    item.Variations.Add(variation);
                }
                order.Items.Add(item);
            }

            yield return order;
        }
    }

    private record DbMenuData(List<Db.ProductSize> ProductSizes, List<Db.Flavour> Flavours, List<Db.Product> Products, List<Db.ProductDetail> ProductDetails, Dictionary<int, string> Departments, List<Db.DealItemDetail> DealItemDetails, List<Db.DealDescription> DealDescriptions, List<Db.Discount> ItemDiscounts, List<Db.DiscountProductDetailMapping> DiscountMappings);
}