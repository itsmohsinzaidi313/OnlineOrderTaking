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
        var gsts = await dbContext.Gsts.ToListAsync();

        foreach (var item in cities)
        {
            var areasJsonArray = new JsonArray();
            foreach (var item1 in areas
                .Where(x => x.CityId == item.CityId)
                .Select(x => new JsonObject()
                {
                    ["AreaId"] = x.AreaId,
                    ["AreaName"] = x.AreaName,
                    ["DeliveryTime"] = branchDetails.Where(bd => bd.AreaId == x.AreaId).Select(bd => bd.DeliveryTime).FirstOrDefault() ?? 0,
                    ["DeliveryCharges"] = branchDetails.Where(bd => bd.AreaId == x.AreaId).Select(bd => bd.DeliveryCharges).FirstOrDefault() ?? 0.00,
                    ["DeliveryChargesWaiveOffLimit"] = branchDetails.Where(bd => bd.AreaId == x.AreaId).Select(bd => bd.DeliveryChargesWaiveOffLimit).FirstOrDefault() ?? 0.00,
                    ["MinimumOrder"] = branchDetails.Where(bd => bd.AreaId == x.AreaId).Select(bd => bd.MinimumOrder).FirstOrDefault() ?? 0.00,
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
                    item2["MinimumOrder"] = branchDetail.MinimumOrder ?? 0.00;
                }
                branchesJsonArray.Add(item2);
            }

            var cityObj2 = new JsonObject
            {
                ["CityName"] = item.CityName,
                ["Branches"] = branchesJsonArray,
                ["Tax"] = gsts.FirstOrDefault(x => x.CityId == item.CityId)?.Gstpercentage ?? 0.00
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

        var orderStatuses = await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
        settingsData["OrderStatuses"] = JsonValue.Create(orderStatuses);
        return settingsData;
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
                    IsPromotional = false,
                    IsPopular = false,
                };
                foreach (var dbProductDetail in dbMenuData.ProductDetails.Where(x => x.ProductId == dbProduct.ProductId))
                {
                    if (dbProductDetail.IsPromotion == true)
                        item.IsPromotional = true;

                    if (dbProductDetail.IsBestSeller == true)
                        item.IsPopular = true;

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
                        Discount = itemDiscount,
                        IsPromotional = dbProductDetail.IsPromotion,
                        IsPopular = dbProductDetail.IsBestSeller,
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

    internal async IAsyncEnumerable<CustomerOrder> GetOrdersAsync(string connectionString, int userId)
    {
        using var dbContext = GetDbContext(connectionString);
        var products = await (from x in dbContext.ProductCategories
                              join y in dbContext.Products on x.CategoryId equals y.ProductCategoryId
                              select y).ToListAsync();
        var productDetails = await (from a in dbContext.ProductDetails
                                    join b in dbContext.Products on a.ProductId equals b.ProductId
                                    join c in dbContext.ProductCategories on b.ProductCategoryId equals c.CategoryId
                                    select a).ToListAsync();
        var dealItems = await (from a in dbContext.DealItemDetails
                               join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                               join c in dbContext.Products on b.ProductId equals c.ProductId
                               join d in dbContext.ProductCategories on c.ProductCategoryId equals d.CategoryId
                               select a).ToListAsync();
        var dealDescriptions = await (from a in dbContext.DealDescriptions
                                      join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                      join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                      join d in dbContext.Products on c.ProductId equals d.ProductId
                                      join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                      select a).ToListAsync();
        var flavours = await dbContext.Flavours.ToDictionaryAsync(x => x.FlavourId, x => x);
        var sizes = await dbContext.ProductSizes.ToDictionaryAsync(x => x.SizeId, x => x);
        var setupDetail = await dbContext.SetupMasterDetails.ToDictionaryAsync(x => x.SetupDetailId, x => x.SetupDetailName);
        var statuses = await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
        var branchDict = await dbContext.BranchMasters.ToDictionaryAsync(x => x.BranchId, x => x.BranchName);
        var discounts = await dbContext.Discounts.ToDictionaryAsync(x => x.DiscountId, x => x);
        var riders = await dbContext.Riders.ToListAsync();

        foreach (var branchId in await dbContext.UserBranchMappings.Where(x => x.UserId == userId).Select(x => x.BranchId).ToListAsync())
        {
            foreach (var dbOrder in await dbContext.OrderMasters.Where(x => x.BranchId == branchId && x.OrderDate > DateOnly.FromDateTime(DateTime.Now.AddDays(-3))).ToListAsync())
            {
                var orderTime = dbOrder.OrderTime;
                var orderDate = dbOrder.OrderDate;
                DateTime? orderDateTime = orderDate?.ToDateTime(orderTime);
                var orderStatusLogs = await dbContext.OrderStatusLogs.Where(x => x.OrderMasterId == dbOrder.OrderMasterId).ToListAsync();

                // Find Asia/Karachi timezone once per order
                var karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");

                var order = new CustomerOrder
                {
                    OrderNumber = dbOrder.OrderNumber ?? "N/A",
                    OrderToken = dbOrder.OrderToken ?? "N/A",
                    BranchId = branchId,
                    BranchName = branchDict[branchId],
                    OrderType = setupDetail[dbOrder.OrderModeId!.Value],
                    Status = statuses[dbOrder.OrderStatusId],
                    Items = [],
                    DeliveryCharges = (int?)(dbOrder.DeliveryCharges ?? 0.00),
                    AmountWithoutGst = dbOrder.TotalAmountWithoutGst ?? 0.00,
                    AmountWithGst = dbOrder.TotalAmountWithGst ?? 0.00,
                    OrderTime = orderDateTime ?? DateTime.MinValue,
                    GstPercentage = dbOrder.Gstpercent,
                    OrderStatusLogs = orderStatusLogs.Select(x => new
                    {
                        Id = x.OrderStatusId,
                        CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.SpecifyKind(x.CreatedDateTime, DateTimeKind.Utc),
                            karachiTz
                        ),
                    }).ToList(),
                    Rider = riders.Select(x => new Rider { Id = x.RiderId, Name = x.RiderName ?? string.Empty, Contact = x.Contact1 ?? string.Empty }).FirstOrDefault(x => x.Id == dbOrder.RiderId),
                    DeliveryTime = dbOrder.DeliveryTime ?? 0,
                    TotalDiscount = dbOrder.DiscountAmount ?? 0.00,

                };
                order.PreviousOrderCount = await dbContext.OrderMasters.Where(x => x.PhoneId == dbOrder.PhoneId).CountAsync();
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
                        DeliveryInstructions = addressDetails?.Remarks ?? "N/A",
                        AlternateMobileNumber = dbOrder.AlternateNumber ?? "N/A",
                        EmailAddress = dbOrder.EmailAddress ?? "N/A",
                        Title = customer.Title ?? "N/A",
                    };
                    order.CustomerDetails = customerDetail;
                }
                await foreach (var item in GetOrderItemsAsync(dbContext, dbOrder.OrderMasterId, productDetails, dealItems, products, flavours, sizes, dealDescriptions, discounts))
                {
                    item.Price = item.Variations.Sum(x => x.Price + x.ItemChoices.SelectMany(y => y.ItemOptions).Sum(z => z.Price));
                    order.Items.Add(item);
                }

                yield return order;
            }
        }
    }

    internal async Task<Dictionary<int, string>> GetOrderStatusesAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        return await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
    }

    private async IAsyncEnumerable<MenuItem> GetOrderItemsAsync(Db.PgDbContext dbContext, int orderMasterId, List<Db.ProductDetail> productDetails, List<Db.DealItemDetail> dealItems, List<Db.Product> products, Dictionary<int, Db.Flavour> flavours, Dictionary<int, Db.ProductSize> sizes, List<Db.DealDescription> dealDescriptions, Dictionary<int, Db.Discount> discounts)
    {
        var orderDetails = await dbContext.OrderDetails
            .Where(x => x.OrderMasterId == orderMasterId && x.IsActive == true)
            .ToListAsync() ?? [];
        var categories = await (from x in dbContext.ProductCategories
                                join y in dbContext.Products on x.CategoryId equals y.ProductCategoryId
                                select new { y.ProductId, x.CategoryId }).ToDictionaryAsync(x => x.ProductId, x => x.CategoryId);

        foreach (var orderDetail in orderDetails.Where(x => !x.DealItemId.HasValue))
        {
            var productDetail = productDetails.Where(x => x.ProductDetailId == orderDetail.ProductDetailId).First();
            var product = products.Where(x => x.ProductId == productDetail.ProductId).First();
            var flavour = flavours[productDetail.FlavourId!.Value];
            var size = sizes[productDetail.SizeId];
            var dealItemIdList = orderDetails
                        .Where(x => x.OrderParentId == orderDetail.ProductDetailId)
                        .Select(x => x.DealItemId)
                        .Distinct();

            Discount? discount1 = null;
            if (orderDetail.DiscountId.HasValue && orderDetail.DiscountId != 0)
            {
                var dbDiscount = discounts[orderDetail.DiscountId.Value];
                discount1 = new Discount
                {
                    Id = dbDiscount.DiscountId,
                    Name = dbDiscount.DiscountName ?? "N/A",
                    MaxCap = decimal.ToDouble(dbDiscount.DiscountCapEnd),
                    MinCap = decimal.ToDouble(dbDiscount.DiscountCapStart),
                    Type = PointofSaleModels.Application.ValueType.Percentage.ToString(),
                    Value = dbDiscount.DiscountPercent,
                };
            }
            yield return new MenuItem
            {
                Id = product.ProductId,
                Name = product.ProductName ?? "N/A",
                Image = product.ProductImage ?? "N/A",
                CategoryId = categories[product.ProductId].ToString(),
                IsKot = orderDetail.IsKot,
                Quantity = orderDetail.Quantity ?? 0,
                Variations =
                [
                    new ()
                        {
                            Id = productDetail.ProductDetailId,
                            Discount = discount1,
                            Size = new ItemSize
                            {
                                Id = size.SizeId,
                                Name = size.SizeName ?? "N/A",
                            },
                            Flavour = new ItemFlavour
                            {
                                Id = flavour.FlavourId,
                                Name = flavour.FlavourName ?? "N/A",
                            },
                            Price = productDetail.Price,
                            ItemChoices = [.. dealItems.Where(x => dealItemIdList.Contains(x.DealItemId))

                            .Select(x => new ItemChoice{
                                Id  = x.DealItemId,
                                MaxChoice = x.MaxQuantity ?? 0,
                                Quantity = x.Quantity ?? 0,
                                Name = x.DealOptionName,
                                ItemOptions = [..orderDetails
                                .Where(y => y.OrderParentId == orderDetail.ProductDetailId && y.DealItemId == x.DealItemId)
                                .Select(y => new ItemOption {
                                    Id = y.ProductDetailId,
                                    Name = (from a in productDetails
                                            join b in products on a.ProductId equals b.ProductId
                                            where a.ProductDetailId == y.ProductDetailId
                                            select b.ProductName).FirstOrDefault() ?? "",
                                    Price = (from a in dealDescriptions
                                             where a.ProductDetailId == y.ProductDetailId && a.DealItemId == x.DealItemId
                                             select a.Price).FirstOrDefault() ?? 0.0,
                                    Quantity = (int?)(y.Quantity ?? 0.00)
                                })]
                            })],
                        }
                ]
            };
        }
    }

    internal async Task<object> GetRidersAsync(int userId, string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var list = await dbContext.Riders
            .Join(dbContext.UserBranchMappings, a => a.BranchId, b => b.BranchId, (a, b) => new { Riders = a, b.UserId })
            .Where(x => x.UserId == userId)
            .Select(x => new Rider
            {
                Id = x.Riders.RiderId,
                Name = x.Riders.RiderName,
                Contact = x.Riders.Contact1
            })
            .ToListAsync();
        return list;
    }

    internal async Task<object> GetBranchesAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var list = await dbContext.BranchMasters
            .Where(x => x.IsActive)
            .Select(x => new
            {
                Id = x.BranchId,
                Name = x.BranchName
            })
            .ToListAsync();
        return list;
    }

    private record DbMenuData(List<Db.ProductSize> ProductSizes, List<Db.Flavour> Flavours, List<Db.Product> Products, List<Db.ProductDetail> ProductDetails, Dictionary<int, string> Departments, List<Db.DealItemDetail> DealItemDetails, List<Db.DealDescription> DealDescriptions, List<Db.Discount> ItemDiscounts, List<Db.DiscountProductDetailMapping> DiscountMappings);
}