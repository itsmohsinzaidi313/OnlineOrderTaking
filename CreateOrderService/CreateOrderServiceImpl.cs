using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Db = PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Protos;
using static PointofSaleModels.Protos.CreateOrderService;
using PointofSaleModels.Application;
using System.Text.Json;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using PointofSaleModels.ServicePayloads;

namespace CreateOrderService
{
    public class CreateOrderServiceImpl : CreateOrderServiceBase
    {
        private readonly JsonSerializerOptions options;
        private readonly IDbContextFactory<RestaurantsContext> contextFactory;
        private readonly Implementation impl;
        private readonly RabbitMqPublisher publisher;

        public CreateOrderServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, Implementation impl, RabbitMqPublisher publisher)
        {
            this.contextFactory = contextFactory;
            this.impl = impl;
            this.publisher = publisher;
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var order = DeserializeJson(request.OrderJson);
            var connectionString = await GetConnectionString(order.Domain);
            try
            {
                await impl.SaveOrderAsync(connectionString, order!);
                var orderToken = order?.OrderToken ?? throw new Exception("Order token not generated");
                order.OrderStatusLogs = await impl.OrderStatusLogs(connectionString, orderToken);
                if (orderToken == null)
                {
                    return new PlaceOrderResponse { Success = false, Message = "Failed to place order" };
                }
                await NotifyServices(connectionString, order);
                var response = new PlaceOrderResponse { Success = true, OrderNumber = orderToken, Message = "Order placed successfully" };
                return response;
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new PlaceOrderResponse { Success = false, Message = $"Error placing order: {message}" };
            }
        }

        public override async Task<PlaceOrderLegacyResponse> PlaceOrderLegacy(PlaceOrderRequest request, ServerCallContext context)
        {
            var placeOrderResponse = await PlaceOrder(request, context);
            var order = DeserializeJson(request.OrderJson);
            var connectionString = await GetConnectionString(order.Domain);
            var legacyResponse = await impl.GetLegacyResponse(placeOrderResponse.OrderNumber, connectionString);
            return new PlaceOrderLegacyResponse
            {
                Message = placeOrderResponse.Message,
                Success = placeOrderResponse.Success,
                ResponseJson = JsonSerializer.Serialize(legacyResponse)
            };
        }

        private async Task NotifyServices(string connectionString, CustomerOrder order)
        {
            var requestPayload = new OrderServicePayload
            {
                BranchId = order.BranchId,
                RestaurantId = 0,
                DomainName = order.Domain,
                ResponseKey = "CreateOrderResponse",
                SignalRMethodName = "PlaceOrder"
            };
            await foreach (var userId in impl.GetBranchUsersIdsAsync(connectionString, requestPayload.BranchId))
            {
                await publisher.PublishToQueueAsync(RabbitMqQueues.PushNotificationRequestQueue, new PushNotificationServicePayload
                {
                    ClientId = $"branch:{userId}:*",
                    Title = "New Order Received!",
                    Message = $" New order received from the {order?.BranchName} branch - Order# {order?.OrderToken} — Rs.{double.Round(order?.AmountWithGst ?? 0.0 + order.DeliveryCharges ?? 0)}.",
                });

            }
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderHistoryRequestQueue,
                   new DataServicePayload(requestPayload)
                   {
                       OrderToken = order.OrderToken
                   });
            await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
            {
                ExportType = "NewOrder",
                OrderNumber = order.OrderToken,
            });
        }

        private CustomerOrder DeserializeJson(string orderJson)
        {
            var order = JsonSerializer.Deserialize<CustomerOrder>(orderJson, options);
            return order!;
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            var connectionString = restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
            return connectionString.Replace("5434", "5433");
        }
    }
}
