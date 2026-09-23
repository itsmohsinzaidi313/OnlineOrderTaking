using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Integrations;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Net.Http.Headers;
using Db = PointofSaleModels.PGDatabaseModels;

namespace FoodpandaOrderService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
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

                var restaurantsContext = contextFactory.CreateDbContext();
                var domain = requestPayload.RemoteId switch
                {
                    "POS123" => "ginsoy.eatx.pk",
                    _ => throw new Exception("Unknown order code")
                };
                var restaurant = await restaurantsContext.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domain) ?? throw new Exception("Restaurant not found");
                var connString = restaurant.ConnectionString;
                var orderNumber = await SaveToDatabase(connString, order) ?? throw new Exception("Order cannot be saved");
                logger.LogInformation("Order Saved {orderNumber}", orderNumber);

                var url = order?.CallbackUrls?.OrderAcceptedUrl ?? throw new Exception("Order accepted URL is missing");
                var orderCode = order.Code ?? throw new Exception("Order code is missing");
                var accessToken = await RequestAccessTokenAsync() ?? throw new Exception("Access token is missing");
                await OrderAcceptedStatus(accessToken, orderCode, url.ToString());
                logger.LogInformation("Acknowledgement sent to FP for {orderNumber}", orderNumber);
                using var dbContext = GetDbContext(connString);
                foreach (var userId in await dbContext.UserLogins.Where(x => x.CompanyId == 1193).ToListAsync())
                {
                    await publisher.PublishToQueueAsync(RabbitMqQueues.PushNotificationRequestQueue, new PushNotificationServicePayload
                    {
                        ClientId = $"branch:{userId}:*",
                        Title = "New Order Received!",
                        Message = $" New order received from the DHA BRANCH branch - Order# {orderNumber} — Rs.{decimal.Round(order?.Price.SubTotal ?? 0.00M + decimal.Parse(order.Price.DeliveryFee ?? "0.0"))}.",
                    });

                }
                await publisher.PublishToQueueAsync(RabbitMqQueues.OrderHistoryRequestQueue,
                   new DataServicePayload(requestPayload)
                   {
                       OrderToken = orderNumber
                   });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
        }

        private static async Task<string?> SaveToDatabase(string connectionString, FoodPandaPayloadModel order)
        {
            using var dbContext = GetDbContext(connectionString);
            var exists = await dbContext.OrderMasters.AsNoTracking().Where(x => $"{order.Code}/${order.ShortCode}" == x.OrderNumber).AnyAsync();
            if (exists) return $"{order.Code}/${order.ShortCode}";
            var strategy = dbContext.Database.CreateExecutionStrategy();
            var companyId = await dbContext.SetupCompanies.AsNoTracking().Select(x => x.CompanyId).FirstOrDefaultAsync();
            var branchId = await dbContext.BranchMasters.AsNoTracking().Select(x => x.BranchId).FirstOrDefaultAsync();
            var areas = await dbContext.Areas.AsNoTracking().ToListAsync();
            var products = await dbContext.ProductDetails.AsNoTracking()
                .ToListAsync();
            var dealDescriptions = await dbContext.DealItemDetails.AsNoTracking()
                .Where(x => x.IsActive == true)
                .ToListAsync();
            return await strategy.ExecuteAsync(
                order,
                async (context, orderData, ct) =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                    try
                    {
                        var customer = orderData.Customer;
                        var customerPhone = customer?.MobilePhone.Replace("+92", "0");
                        var customerPhoneId = (await dbContext.CustomerPhones.FirstOrDefaultAsync(x => x.PhoneNumber == customerPhone, ct))?.PhoneId;
                        if (customerPhoneId == null)
                        {
                            var newCustomerPhone = new Db.CustomerPhone
                            {
                                CompanyId = companyId,
                                PhoneNumber = customerPhone,
                                IsActive = true,
                            };
                            await dbContext.CustomerPhones.AddAsync(newCustomerPhone);
                            await dbContext.SaveChangesAsync();
                            customerPhoneId = newCustomerPhone.PhoneId;
                        }
                        var dbCustomer = await dbContext.Customers.Where(x => x.PhoneId == customerPhoneId).FirstOrDefaultAsync();
                        var customerName = $"{customer?.FirstName} {customer?.LastName}";
                        var customerId = dbCustomer?.CustomerId ?? 0;
                        if (dbCustomer == null)
                        {
                            var newCustomer = new Db.Customer
                            {
                                CompanyId = companyId,
                                PhoneId = customerPhoneId,
                                CustomerName = customerName,
                                Email = customer?.Email,
                                IsActive = true,
                            };
                            await dbContext.Customers.AddAsync(newCustomer);
                            await dbContext.SaveChangesAsync();
                            customerId = newCustomer.CustomerId;
                        }

                        var address = orderData.Delivery.Address;
                        var dbCity = await dbContext.Cities.Select(x => new { x.CityId, x.CityName }).Where(x => x.CityName.Contains(address.City)).FirstOrDefaultAsync();
                        var fullAddress = $"{address.Room} {address.FlatNumber} {address.Number} {address.Floor} {address.Building} {address.Street} {address.DeliveryMainArea} {address.City}";
                        var dbCustomerAddress = await dbContext.CustomerAddressDetails.Where(x => x.CompleteAddress == fullAddress).FirstOrDefaultAsync();
                        var customerAddressId = dbCustomerAddress?.CustomerAddressId ?? 0;
                        if (dbCustomerAddress == null)
                        {
                            var newCustomerAddress = new Db.CustomerAddressDetail
                            {
                                CompanyId = companyId,
                                PhoneId = customerPhoneId,
                                CompleteAddress = fullAddress,
                                CityId = dbCity?.CityId ?? 0,
                                IsActive = true,
                            };
                            await dbContext.CustomerAddressDetails.AddAsync(newCustomerAddress);
                            await dbContext.SaveChangesAsync();
                            customerAddressId = newCustomerAddress.CustomerAddressId;
                        }
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
                        var gstFactor = gst?.Gstpercentage ?? 1 / 100;
                        var subTotal = decimal.ToDouble(orderData.Price.SubTotal);
                        var orderTypeDescription = orderData.ExpeditionType switch
                        {
                            "delivery" => "DELIVERY",
                            "pickup" => "TAKE AWAY",
                            _ => "Unknown"
                        };
                        var orderType = await dbContext.SetupMasterDetails.FirstOrDefaultAsync(x => x.Flex1 == orderTypeDescription);
                        var orderstatus = await dbContext.OrderStatuses.Where(x => x.OrderStatusName == "Confirmed").FirstOrDefaultAsync();
                        var orderSource = await dbContext.SetupMasterDetails.Where(x => x.CompanyId == companyId && x.Flex1 == "WEB").FirstOrDefaultAsync();
                        var areaId = 0;
                        var addr = address.DeliveryMainArea.ToLower();
                        foreach (var area in areas)
                        {
                            if (addr.Contains(area.AreaName.ToLower()))
                            {
                                areaId = area.AreaId;
                                break;
                            }
                        }
                        var orderMaster = new Db.OrderMaster
                        {
                            CompanyId = companyId,
                            BranchId = branchId,
                            AreaId = 0,
                            IsActive = true,
                            SpecialInstruction = orderData.Comments?.CustomerComment,
                            PaymentTermId = paymentTermId,
                            OrderNumber = $"{orderData.Code}/${orderData.ShortCode}",
                            Gstid = gst?.Gstid,
                            Gstpercent = gst?.Gstpercentage ?? 0.00,
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
                            OrderStatusId = orderstatus!.OrderStatusId,
                            OrderSourceId = orderSource!.SetupDetailId,
                            PaymentTypeId = paymentModeId,
                            PhoneId = customerPhoneId,
                            CustomerId = customerId,
                            CustomerAddressId = customerAddressId,
                        };
                        foreach (var product in orderData.Products ?? [])
                        {
                            var remoteCode = 0;
                            if (product.RemoteCode.Contains('|'))
                            {
                                remoteCode = int.Parse(product.RemoteCode.Replace("prd", "").Split("|").Last());
                            }
                            else
                            {
                                remoteCode = int.Parse(product.RemoteCode.Replace("prd", ""));
                            }
                            var pd = products.FirstOrDefault(p => p.ProductDetailId == remoteCode) ?? throw new Exception("Product not found");

                            List<Db.OrderDetail> orderDetails = [];
                            var orderDetail = new Db.OrderDetail
                            {
                                OrderMasterId = orderMaster.OrderMasterId,
                                ProductDetailId = pd.ProductDetailId,
                                IsActive = true,
                                PriceWithoutGst = pd.Price,
                                PriceWithGst = pd.Price + (pd.Price * gstFactor),
                                Gstid = gst?.Gstid,
                                Quantity = (int)product.Quantity,
                                SpecialInstruction = product.Comment,
                                RandomId = new Random().Next(8999) + 1000,
                            };
                            orderDetails.Add(orderDetail);
                            foreach (var tpId in product.SelectedToppings)
                            {
                                var dealProductDetailId = 0;
                                if (product.RemoteCode.Contains("|"))
                                {
                                    dealProductDetailId = int.Parse(tpId.RemoteCode.Replace("prd", "").Split("|").Last());
                                }
                                else
                                {
                                    dealProductDetailId = int.Parse(tpId.RemoteCode.Replace("prd", ""));
                                }
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
                                    Gstid = gst?.Gstid,
                                    PriceWithoutGst = double.Parse(tpId.Price ?? "0.00"),
                                    PriceWithGst = double.Parse(tpId.Price ?? "0.00") + (double.Parse(tpId.Price ?? "0.00") * gstFactor),
                                });
                            }

                            orderMaster.OrderDetails.Add(orderDetail);
                        }

                        await dbContext.OrderMasters.AddAsync(orderMaster, ct);
                        await dbContext.SaveChangesAsync(ct);
                        //throw new Exception("Test exception to trigger rollback"); // Remove this line in production
                        await transaction.CommitAsync(ct);
                        return orderMaster.OrderNumber;
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
