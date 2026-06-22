using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Integrations;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Net.Http.Headers;
using Db = PointofSaleModels.PGDatabaseModels;

namespace FoodpandaIntegrationService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.FoodpandaIntegrationRequestQueue;
        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<IntegrationServicePayload<FoodPandaPayloadModel>>(transport);
            object? response = null;
            try
            {
                var order = requestPayload?.OrderPayload ?? throw new Exception("Order payload is missing");
                var url = order?.CallbackUrls?.OrderAcceptedUrl ?? throw new Exception("Order accepted URL is missing");
                var orderCode = order.Code ?? throw new Exception("Order code is missing");
                var accessToken = await RequestAccessTokenAsync() ?? throw new Exception("Access token is missing");

                await OrderAcceptedStatus(accessToken, orderCode, url.ToString());

                await SaveToDatabase(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
        }

        private static async Task SaveToDatabase(FoodPandaPayloadModel order)
        {
            var dbContext = GetDbContext("");
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(
                order,
                async (context, orderData, ct) =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                    try
                    {
                        var orderMaster = new Db.OrderMaster
                        {
                            SpecialInstruction = orderData.Comments?.CustomerComment,
                        };

                        foreach (var product in orderData.Products ?? [])
                        {
                            var orderDetail = new Db.OrderDetail
                            {
                                ProductDetailId = int.Parse(product.Id.ToString()),
                                IsActive = true,
                                PriceWithoutGst = (double)product.UnitPrice,
                                Quantity = (int)product.Quantity,
                                SpecialInstruction = product.Comment
                            };
                            orderMaster.OrderDetails.Add(orderDetail);
                        }

                        var customerPhone = await dbContext.CustomerPhones.FirstOrDefaultAsync(x => x.PhoneNumber == orderData.Customer.MobilePhone, ct);

                        if (customerPhone != null)
                        {
                            var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneId == customerPhone.PhoneId, ct);
                        }

                        await dbContext.OrderMasters.AddAsync(orderMaster, ct);
                        await dbContext.SaveChangesAsync(ct);
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
