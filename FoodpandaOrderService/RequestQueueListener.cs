using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using PointofSaleModels.Integrations;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Net.Http.Headers;
using Db = PointofSaleModels.PGDatabaseModels;

namespace FoodpandaOrderService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.FoodpandaIntegrationRequestQueue;
        public override async Task OnMessage(string transport)
        {
            logger.LogInformation(transport);
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<IntegrationServicePayload<FoodPandaPayloadModel>>(transport);
            object? response = null;
            try
            {
                var order = requestPayload?.OrderPayload ?? throw new Exception("Order payload is missing");
                Console.WriteLine($"Received order request: {order.Code}\n{requestPayload}");
                var url = order?.CallbackUrls?.OrderAcceptedUrl ?? throw new Exception("Order accepted URL is missing");
                var orderCode = order.Code ?? throw new Exception("Order code is missing");
                var accessToken = await RequestAccessTokenAsync() ?? throw new Exception("Access token is missing");

                await OrderAcceptedStatus(accessToken, orderCode, url.ToString());
                var restaurantsContext = contextFactory.CreateDbContext();
                var domain = requestPayload.RemoteId switch
                {
                    "POS123" => "pathan.eatx.pk",
                    _ => throw new Exception("Unknown order code")
                };
                var restaurant = await restaurantsContext.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domain) ?? throw new Exception("Restaurant not found");

                await SaveToDatabase(restaurant.ConnectionString.Replace("haproxy", "localhost"), order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
        }

        private static async Task SaveToDatabase(string connectionString, FoodPandaPayloadModel order)
        {
            var dbContext = GetDbContext(connectionString);
            var strategy = dbContext.Database.CreateExecutionStrategy();
            var companyId = await dbContext.SetupCompanies.Select(x => x.CompanyId).FirstOrDefaultAsync();
            var branchId = await dbContext.BranchMasters.Select(x => x.BranchId).FirstOrDefaultAsync();
            var itemIds = order.Products?.Select(x => int.Parse(x.Id.ToString())).ToList() ?? [];

            var products = await dbContext.ProductDetails
                .Where(x => itemIds.Contains(x.ProductDetailId))
                .ToListAsync();
            var dealDescriptions = await dbContext.DealItemDetails
                .Where(x => x.IsActive == true)
                .ToListAsync();

            await strategy.ExecuteAsync(
                order,
                async (context, orderData, ct) =>
                {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                try
                {
                    var customer = orderData.Customer;
                    var customerPhone = customer?.MobilePhone.Replace("+92", "0");
                    var customerId = (await dbContext.CustomerPhones.FirstOrDefaultAsync(x => x.PhoneNumber == customerPhone, ct))?.PhoneId;
                    var customerName = $"{customer?.FirstName} {customer?.LastName}";
                    var address = orderData.Delivery.Address;
                    var fullAddress = $"{address.Room} {address.FlatNumber} {address.Number} {address.Floor} {address.Building} {address.Street} {address.DeliveryMainArea} {address.City}";
                    var paymentType = orderData.Payment.Type;
                    var paymentTypeDescription = paymentType switch
                    {
                        "Cash On Delivery" => "By Cash",
                        "Online payment" => "By Card",
                        _ => "Unknown"
                    };
                    var paymentTermId = await dbContext.SetupMasterDetails
                        .Where(x => x.SetupDetailName == paymentTypeDescription && x.CompanyId == companyId)
                        .Select(x => x.SetupDetailId)
                        .FirstOrDefaultAsync(ct);
                    var paymentModeDescription = paymentType switch
                    {
                        "Cash On Delivery" => "CASH",
                        "Online payment" => "CARD",
                        _ => "Unknown"
                    };
                    var paymentModeId = await dbContext.PaymentModes
                        .Where(x => x.PaymentMode1 == paymentModeDescription && x.CompanyId == companyId)
                        .Select(x => x.PaymentModeId)
                        .FirstOrDefaultAsync(ct);
                    var gst = await dbContext.Gsts
                        .Where(x => x.PaymentModeId == paymentModeId && x.CompanyId == companyId)
                        .FirstOrDefaultAsync(ct);
                    var gstFactor = gst.Gstpercentage / 100;
                        var subTotal = decimal.ToDouble(orderData.Price.SubTotal);
                        var orderTypeDescription = orderData.ExpeditionType switch
                        {
                            "Delivery" => "DELIVERY",
                            "Pickup" => "TAKE AWAY",
                            _ => "Unknown"
                        };
                        var orderType = await dbContext.SetupMasterDetails.FirstOrDefaultAsync(x => x.Flex1 == orderTypeDescription);

                        var orderMaster = new Db.OrderMaster
                        {
                            CompanyId = companyId,
                            BranchId = branchId,
                            AreaId = 0,
                            IsActive = true,
                            SpecialInstruction = orderData.Comments?.CustomerComment,
                            PaymentTermId = paymentTermId,
                            OrderNumber = $"{orderData.Code}/${orderData.ShortCode}",
                            Gstid = gst.Gstid,
                            Gstpercent = gst.Gstpercentage ?? 0.00,
                            TotalAmountWithoutGst = subTotal,
                            TotalAmountWithGst = subTotal + (subTotal * gstFactor),
                            AlternateNumber = customerPhone,
                            OrderModeId = orderType.SetupDetailId,
                            OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            OrderTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                            OrderDetails = [],
                            DiscountAmount = 0.00,
                            OrderToken = await GetUniqueTokenAsync(dbContext),
                            Exported = false,
                            PaymentTypeId = paymentModeId
                        };
                        foreach (var product in orderData.Products ?? [])
                        {
                            var remoteCode = int.Parse(product.RemoteCode.Replace("prd", ""));
                            var pd = products.FirstOrDefault(p => p.ProductDetailId == remoteCode) ?? throw new Exception("Product not found");

                            List<Db.OrderDetail> orderDetails = [];
                            var orderDetail = new Db.OrderDetail
                            {
                                OrderMasterId = orderMaster.OrderMasterId,
                                ProductDetailId = pd.ProductDetailId,
                                IsActive = true,
                                PriceWithoutGst = pd.Price,
                                PriceWithGst = pd.Price + (pd.Price * gstFactor),
                                Gstid = gst.Gstid,
                                Quantity = (int)product.Quantity,
                                SpecialInstruction = product.Comment,
                                RandomId = new Random().Next(8999) + 1000,
                            };
                            orderDetails.Add(orderDetail);
                            foreach (var tpId in product.SelectedToppings)
                            {
                                var dealProductDetailId = int.Parse(tpId.RemoteCode.Replace("prd", ""));
                                var dealItemId = dbContext.DealDescriptions.Where(x => x.ProductDetailId == dealProductDetailId).Select(x => x.DealItemId).FirstOrDefault();
                                orderDetails.Add(new Db.OrderDetail
                                {
                                    OrderParentId = orderDetail.ProductDetailId,
                                    RandomId = orderDetail.RandomId,
                                    DealItemId = dealItemId,
                                    ProductDetailId = dealProductDetailId,
                                    Quantity = tpId.Quantity,
                                    IsKot = false,
                                    IsActive = true,
                                    Gstid = gst.Gstid,
                                    PriceWithoutGst = double.Parse(tpId.Price ?? "0.00"),
                                    PriceWithGst = double.Parse(tpId.Price ?? "0.00") + (double.Parse(tpId.Price ?? "0.00") * gstFactor),
                                });
                            }

                            orderMaster.OrderDetails.Add(orderDetail);
                        }

                        await dbContext.OrderMasters.AddAsync(orderMaster, ct);
                        await dbContext.SaveChangesAsync(ct);
                        throw new Exception("Test exception to trigger rollback"); // Remove this line in production
                        await transaction.CommitAsync(ct);

                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                },
                null,
                CancellationToken.None);
        }

        private static async Task<string> GetUniqueTokenAsync(Db.PgDbContext dbContext)
        {
            var token = PointofSaleModels.TokenGenerator.GenerateToken();
            var existingToken = await dbContext.OrderMasters
                .FirstOrDefaultAsync(x => x.OrderToken == token);
            if (existingToken == null)
            {
                return token;
            }
            else
            {
                var newToken = PointofSaleModels.TokenGenerator.GenerateToken();
                return await GetUniqueTokenAsync(dbContext);
            }
        }

        private static async Task OrderAcceptedStatus(string accessToken, string orderCode, string url)
        {
            var content = JsonContent.Create(new
            {
                acceptanceTime = DateTime.Now,
                remoteOrderId = orderCode,
                status = "order_accepted"
            });
            using var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = content,
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST {client.BaseAddress}{request.RequestUri} -> {(int)response.StatusCode} {response.ReasonPhrase}");
        }



        private async Task SaveToken(string domainName, string orderToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            var restaurantId = restaurant?.Id ?? throw new Exception("Restaurant not found");
            var tokenEntity = new Db.OrderTokens { OrderToken = orderToken, CreatedAt = DateTime.UtcNow, RestaurantId = restaurantId };
            await context.OrderTokens.AddAsync(tokenEntity);
            await context.SaveChangesAsync();
        }

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

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }


        private static async Task<string?> RequestAccessTokenAsync()
        {
            const string baseUrl = "https://integration-middleware.as.restaurant-partners.com";
            const string loginPath = "/v2/login";
            const string username = "as-plugin-y-generation-systems-005";
            const string password = "KQ1D8Wcm0M";
            const string secret = "SnyteunCeerhicJofI";

            var apiUrl = $"{baseUrl.TrimEnd('/')}/{loginPath.TrimStart('/')}";
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
    }
}
