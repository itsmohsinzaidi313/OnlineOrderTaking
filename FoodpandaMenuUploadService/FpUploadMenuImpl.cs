using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Protos;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static PointofSaleModels.Protos.FpUploadMenuService;

namespace FoodpandaMenuUploadService
{
    public class FpUploadMenuImpl(IDbContextFactory<SqlServerDbContext> sqlServerDbContextFactory) : FpUploadMenuServiceBase
    {
        const string CallbackUrl = "https://ygensystems.com/api/v2/OnlineOrders/PosIntegration/BBECAFA9-48BA-46BE-A5CF-26E7B0ED76CA";
        const string MenuUrl = "http://85.190.242.39:5019/ExternalMenu";
        const string FPBaseUrl = "https://integration-middleware.as.restaurant-partners.com";
        const string FoodPandaUrl = $"{FPBaseUrl}/v2/chains/Ygen_PK_UAT/catalog";
        public override async Task<FpUploadMenuResponse> UploadMenu(FpUploadMenuRequest request, ServerCallContext context)
        {
            var sqlServerDbContext = await sqlServerDbContextFactory.CreateDbContextAsync();
            var id = sqlServerDbContext.SetupCompanies.Where(x => x.Id == request.Id).Select(x => x.Id).FirstOrDefault();
            if(id == 0)
            {
                return new FpUploadMenuResponse() { Message = "Restaurant not found for the given ID.", Success = false };
            }

            var ygenJson = await GetRestaurantMenu(id);
            if (ygenJson == null)
            {
                return new FpUploadMenuResponse() { Message = "Menu not found for the given restaurant ID.", Success = false };
            }

            var pandaNode = TransformToFoodpanda(ygenJson);
            var response = await SendToFoodpanda(pandaNode);
            return new FpUploadMenuResponse() { Message = response, Success = true };
        }
        private static async Task<JsonNode?> GetRestaurantMenu(int id)
        {
            var httpClient = new HttpClient();
            var body = new
            {
                OperationId = 1,
                CompanyId = id,
                OrderSourceValue = "WEB"
            };
            var content = JsonContent.Create(body);
            var response = await httpClient.PostAsync(MenuUrl, content);
            var ygenJson = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(ygenJson);
        }

        private static async Task<string> SendToFoodpanda(JsonNode manu)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var content = JsonContent.Create(manu, options: options);
            var httpClient = new HttpClient();
            var accessToken = await RequestAccessTokenAsync();
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await httpClient.PutAsync(FoodPandaUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private static async Task<string?> RequestAccessTokenAsync()
        {
            const string loginPath = "/v2/login";
            const string username = "as-plugin-y-generation-systems-005";
            const string password = "KQ1D8Wcm0M";
            const string secret = "SnyteunCeerhicJofI";

            var apiUrl = $"{FPBaseUrl.TrimEnd('/')}/{loginPath.TrimStart('/')}";
            var form = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["grant_type"] = "client_credentials"
            };

            if (!string.IsNullOrWhiteSpace(secret))
            {
                form["secret"] = secret;
            }

            using var content = new FormUrlEncodedContent(form);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginResponse = await client.PostAsync(apiUrl, content);
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            Console.WriteLine($"POST {apiUrl} -> {(int)loginResponse.StatusCode} {loginResponse.ReasonPhrase}");
            if (!string.IsNullOrWhiteSpace(loginContent)) Console.WriteLine(loginContent);

            if (!loginResponse.IsSuccessStatusCode)
            {
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(loginContent);
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                return accessTokenElement.GetString();
            }

            return null;
        }

        private static JsonObject TransformToFoodpanda(JsonNode source)
        {
            var data = source?["Data"] as JsonObject ?? [];
            var products = data["Products"] as JsonArray ?? [];
            var categories = data["Categories"] as JsonArray ?? [];
            var branches = data["Branches"] as JsonArray ?? [];
            var items = new JsonObject();
            var productIdToBaseId = new Dictionary<int, string>();
            var detailIdToVariantId = new Dictionary<int, string>();

            static string BaseProductId(int productId) => $"prd{productId:D5}";
            static string VariantId(string baseId, int detailId) => $"{baseId}|{detailId:D5}";
            static string CategoryId(int categoryId) => $"Category#mcp{categoryId:D5}";
            static string ToppingId(int dealItemId) => $"tt{dealItemId:D5}";
            static string Price(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);

            static string Clean(string? value)
                => string.IsNullOrWhiteSpace(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    ? string.Empty
                    : value.Trim();

            static decimal ReadDecimal(JsonNode? node)
            {
                if (node is not JsonValue v) return 0m;
                if (v.TryGetValue<decimal>(out var d)) return d;
                if (v.TryGetValue<double>(out var db)) return (decimal)db;
                if (v.TryGetValue<int>(out var i)) return i;
                if (v.TryGetValue<long>(out var l)) return l;
                if (v.TryGetValue<string>(out var s) && decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var p)) return p;
                return 0m;
            }

            static (string StartTime, string EndTime) NormalizeMenuTimes(string? start, string? end)
            {
                const string fallbackStart = "10:00:00";
                const string fallbackEnd = "22:00:00";

                if (!TimeSpan.TryParse(start, CultureInfo.InvariantCulture, out var startTs) ||
                    !TimeSpan.TryParse(end, CultureInfo.InvariantCulture, out var endTs))
                {
                    return (fallbackStart, fallbackEnd);
                }

                if (endTs <= startTs)
                {
                    return (fallbackStart, fallbackEnd);
                }

                // Foodpanda rejects full-day windows from some POS feeds.
                if (startTs == TimeSpan.Zero && endTs >= new TimeSpan(23, 59, 0))
                {
                    return (fallbackStart, fallbackEnd);
                }

                return (startTs.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), endTs.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            }

            // Pass 1: collect product/detail id mapping
            foreach (var pNode in products)
            {
                if (pNode is not JsonObject pObj) continue;

                var productId = pObj["ProductId"]?.GetValue<int>() ?? 0;
                if (productId == 0) continue;

                var baseId = BaseProductId(productId);
                productIdToBaseId[productId] = baseId;

                var details = pObj["ProductDetails"] as JsonArray ?? [];
                foreach (var dNode in details)
                {
                    if (dNode is not JsonObject dObj) continue;
                    var detailId = dObj["ProductDetailId"]?.GetValue<int>() ?? 0;
                    if (detailId == 0) continue;
                    detailIdToVariantId[detailId] = VariantId(baseId, detailId);
                }
            }

            var createdToppings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var nextToppingOrder = 1;

            // Pass 2: build products + variants + toppings
            foreach (var pNode in products)
            {
                if (pNode is not JsonObject pObj) continue;

                var productId = pObj["ProductId"]?.GetValue<int>() ?? 0;
                if (productId == 0 || !productIdToBaseId.TryGetValue(productId, out var baseId)) continue;

                var productName = Clean(pObj["ProductName"]?.GetValue<string>());
                var productDescription = Clean(pObj["ProductDescription"]?.GetValue<string>());
                var productEnabled = pObj["IsEnable"]?.GetValue<bool>() ?? false;

                var details = pObj["ProductDetails"] as JsonArray ?? [];
                var variantsObj = new JsonObject();
                var hasActiveVariant = false;

                foreach (var dNode in details)
                {
                    if (dNode is not JsonObject dObj) continue;

                    var detailId = dObj["ProductDetailId"]?.GetValue<int>() ?? 0;
                    if (detailId == 0) continue;

                    var variantId = VariantId(baseId, detailId);
                    var detailEnabled = dObj["IsEnable"]?.GetValue<bool>() ?? false;
                    hasActiveVariant |= detailEnabled;

                    variantsObj[variantId] = new JsonObject
                    {
                        ["id"] = variantId,
                        ["type"] = "Product"
                    };

                    var variantTitle = productName;
                    var sizeName = Clean(dObj["SizeName"]?.GetValue<string>());
                    if (!string.IsNullOrEmpty(sizeName) && sizeName != "-")
                        variantTitle = $"{productName}, {sizeName}";

                    var toppingsRef = new JsonObject();
                    var dealItems = dObj["DealItems"] as JsonArray ?? [];

                    foreach (var diNode in dealItems)
                    {
                        if (diNode is not JsonObject diObj) continue;

                        var dealItemId = diObj["DealItemId"]?.GetValue<int>() ?? 0;
                        if (dealItemId == 0) continue;

                        var toppingId = ToppingId(dealItemId);
                        toppingsRef[toppingId] = new JsonObject
                        {
                            ["id"] = toppingId,
                            ["type"] = "Topping"
                        };

                        if (createdToppings.Contains(toppingId))
                            continue;

                        createdToppings.Add(toppingId);

                        var minQ = diObj["MinQuantity"]?.GetValue<int>() ?? 0;
                        var maxQRaw = diObj["MaxQuantity"]?.GetValue<int>() ?? 0;

                        var dealDescriptions = diObj["DealDescriptions"] as JsonArray ?? [];
                        var maxQ = maxQRaw > 0 ? maxQRaw : dealDescriptions.Count;

                        var toppingProducts = new JsonObject();

                        foreach (var ddNode in dealDescriptions)
                        {
                            if (ddNode is not JsonObject ddObj) continue;

                            var dealProductDetailId = ddObj["DealProductDetailId"]?.GetValue<int>() ?? 0;
                            var productRefId = detailIdToVariantId.TryGetValue(dealProductDetailId, out var mappedVariantId)
                                ? mappedVariantId
                                : $"prdX|{dealProductDetailId:D5}";

                            if (!items.ContainsKey(productRefId) && !detailIdToVariantId.ContainsKey(dealProductDetailId))
                            {
                                var fallbackName = Clean(ddObj["ProductName"]?.GetValue<string>());
                                var fallbackPrice = ReadDecimal(ddObj["Price"]);

                                items[productRefId] = new JsonObject
                                {
                                    ["id"] = productRefId,
                                    ["type"] = "Product",
                                    ["title"] = new JsonObject { ["default"] = fallbackName },
                                    ["description"] = new JsonObject { ["default"] = fallbackName },
                                    ["price"] = Price(fallbackPrice),
                                    ["active"] = true,
                                    ["isPrepackedItem"] = false,
                                    ["isExpressItem"] = false,
                                    ["excludeDishInformation"] = false
                                };
                            }

                            toppingProducts[productRefId] = new JsonObject
                            {
                                ["id"] = productRefId,
                                ["type"] = "Product",
                                ["price"] = Price(ReadDecimal(ddObj["Price"]))
                            };
                        }

                        items[toppingId] = new JsonObject
                        {
                            ["id"] = toppingId,
                            ["type"] = "Topping",
                            ["order"] = nextToppingOrder++,
                            ["title"] = new JsonObject { ["default"] = Clean(diObj["DealOptionName"]?.GetValue<string>()) },
                            ["quantity"] = new JsonObject
                            {
                                ["minimum"] = minQ,
                                ["maximum"] = Math.Max(minQ, maxQ)
                            },
                            ["products"] = toppingProducts
                        };
                    }

                    var variantObj = new JsonObject
                    {
                        ["id"] = variantId,
                        ["title"] = new JsonObject { ["default"] = variantTitle },
                        ["type"] = "Product",
                        ["price"] = Price(ReadDecimal(dObj["Price"])),
                        ["parent"] = new JsonObject
                        {
                            ["id"] = baseId,
                            ["type"] = "Product"
                        },
                        ["active"] = productEnabled && detailEnabled,
                        ["isPrepackedItem"] = false,
                        ["isExpressItem"] = false,
                        ["excludeDishInformation"] = false
                    };

                    if (toppingsRef.Count > 0)
                        variantObj["toppings"] = toppingsRef;

                    items[variantId] = variantObj;
                }

                items[baseId] = new JsonObject
                {
                    ["id"] = baseId,
                    ["type"] = "Product",
                    ["title"] = new JsonObject { ["default"] = productName },
                    ["description"] = new JsonObject { ["default"] = productDescription },
                    ["variants"] = variantsObj,
                    ["active"] = productEnabled && hasActiveVariant,
                    ["isPrepackedItem"] = false,
                    ["isExpressItem"] = false,
                    ["excludeDishInformation"] = false
                };
            }

            // Categories
            foreach (var cNode in categories)
            {
                if (cNode is not JsonObject cObj) continue;

                var categoryId = cObj["CategoryId"]?.GetValue<int>() ?? 0;
                if (categoryId == 0) continue;

                var catItemId = CategoryId(categoryId);
                var catProducts = new JsonObject();

                foreach (var pNode in products)
                {
                    if (pNode is not JsonObject pObj) continue;

                    var pCategoryId = pObj["ProductCategoryId"]?.GetValue<int>() ?? 0;
                    if (pCategoryId != categoryId) continue;

                    var pId = pObj["ProductId"]?.GetValue<int>() ?? 0;
                    if (pId == 0 || !productIdToBaseId.TryGetValue(pId, out var baseId)) continue;

                    catProducts[baseId] = new JsonObject
                    {
                        ["id"] = baseId,
                        ["type"] = "Product"
                    };
                }

                items[catItemId] = new JsonObject
                {
                    ["id"] = catItemId,
                    ["type"] = "Category",
                    ["title"] = new JsonObject { ["default"] = Clean(cObj["CategoryName"]?.GetValue<string>()) },
                    ["description"] = new JsonObject { ["default"] = Clean(cObj["CategoryName"]?.GetValue<string>()) },
                    ["products"] = catProducts
                };
            }

            // One schedule + one delivery menu
            var startTime = "00:00:00";
            var endTime = "23:59:59";

            if (branches.FirstOrDefault() is JsonObject firstBranch &&
                firstBranch["OperatingDays"] is JsonArray opDays &&
                opDays.FirstOrDefault() is JsonObject firstDay)
            {
                startTime = firstDay["StartTime"]?.GetValue<string>() ?? startTime;
                endTime = firstDay["EndTime"]?.GetValue<string>() ?? endTime;
            }

            (startTime, endTime) = NormalizeMenuTimes(startTime, endTime);

            const string scheduleId = "schedule00001";
            items[scheduleId] = new JsonObject
            {
                ["id"] = scheduleId,
                ["type"] = "ScheduleEntry",
                ["startTime"] = startTime,
                ["endTime"] = endTime
            };

            var menuProducts = new JsonObject();
            foreach (var kv in productIdToBaseId)
            {
                menuProducts[kv.Value] = new JsonObject
                {
                    ["id"] = kv.Value,
                    ["type"] = "Product"
                };
            }

            items["m00001"] = new JsonObject
            {
                ["id"] = "m00001",
                ["title"] = new JsonObject { ["default"] = "Regular Menu" },
                ["description"] = new JsonObject { ["default"] = "Regular Menu" },
                ["type"] = "Menu",
                ["menuType"] = "DELIVERY",
                ["schedule"] = new JsonObject
                {
                    [scheduleId] = new JsonObject
                    {
                        ["id"] = scheduleId,
                        ["type"] = "ScheduleEntry"
                    }
                },
                ["products"] = menuProducts
            };

            var vendors = new JsonArray() { "POS123" };

            return new JsonObject
            {
                ["callbackUrl"] = CallbackUrl,
                ["catalog"] = new JsonObject
                {
                    ["items"] = items
                },
                ["vendors"] = vendors
            };
        }
    }
}
