using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace SettingsDataService;

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
            .Join(dbContext.SetupCompanySettings, a => a.SetupDetailId, b => b.SetupDetailId, (a, b) => new { Key = a.Flex1 ?? "", Value = b.SettingValue ?? "" })
            .Where(x => keys.Contains(x.Key))
            .ToDictionaryAsync(x => x.Key, x => x.Value);

        var restaurantLogo = settings.GetValueOrDefault("UPLOAD_LOGO", string.Empty);
        var splashBanner = settings.GetValueOrDefault("UPLOAD_SPLASH_BANNER", string.Empty);
        var websiteBackgroundImage = settings.GetValueOrDefault("UPLOAD_BACKGROUND", string.Empty);

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
}