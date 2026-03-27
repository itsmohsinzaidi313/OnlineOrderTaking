using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderServicePayload>(transport);
            object? response = null;
            try
            {
                if (requestPayload == null)
                {
                    logger.LogWarning("Invalid or missing order payload for company {CompanyId}, branch {BranchId}", requestPayload?.RestaurantId, requestPayload?.BranchId);
                    throw new InvalidOperationException("Invalid order payload");
                }
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                connectionString = connectionString.Replace("5434", "5433");
                await impl.SaveOrderAsync(connectionString, requestPayload.Order!);
                var orderToken = requestPayload.Order.OrderToken ?? throw new Exception("Order token not generated");
                await SaveToken(requestPayload.DomainName, orderToken);
                requestPayload.Order.OrderStatusLogs = await impl.OrderStatusLogs(connectionString, orderToken);
                response = new { Success = true, Message = "Order processed successfully", OrderNumber = orderToken };
                await foreach (var userId in impl.GetBranchUsersIdsAsync(connectionString, requestPayload.BranchId))
                {
                    var order = requestPayload.Order;
                    await publisher.PublishToQueueAsync(RabbitMqQueues.PushNotificationRequestQueue, new PushNotificationServicePayload
                    {
                        ClientId = $"branch:{userId}:*",
                        Title = "New Order Received!",
                        Message = $" New order received from the {order?.BranchName} branch - Order# {order?.OrderToken} — Rs.{double.Round(order?.AmountWithGst ?? 0.0 + order.DeliveryCharges ?? 0)}.",
                    });

                }
                await publisher.PublishToQueueAsync(RabbitMqQueues.ClientNotificationRequestQueue,
                    new ClientNotificationServicePayload(requestPayload)
                    {
                        CustomerOrder = requestPayload.Order!,
                    });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
            response = new OrderServicePayload(requestPayload)
            {
                DataPayload = response
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderResponseQueue, response);
            await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
            {
                OrderToken = requestPayload.Order?.OrderToken ?? string.Empty,
            });
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

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
    }
}
