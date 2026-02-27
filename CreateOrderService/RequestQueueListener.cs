using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Microsoft.EntityFrameworkCore;
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
                var orderToken = await impl.SaveOrderAsync(connectionString, requestPayload.BranchId, requestPayload.Order!);
                requestPayload.Order.OrderStatusLogs = await impl.OrderStatusLogs(connectionString, orderToken);
                response = new { Success = true, Message = "Order processed successfully", OrderNumber = orderToken, requestPayload.Order };
                await foreach (var userId in impl.GetBranchUsersIdsAsync(connectionString, requestPayload.BranchId))
                {
                    await publisher.PublishToQueueAsync(RabbitMqQueues.PushNotificationRequestQueue, new PushNotificationServicePayload
                    {
                        ClientId = $"branch:{userId}:*",
                        Title = "New Order Received",
                        Message = $"{orderToken}",
                    });

                }
                await publisher.PublishToQueueAsync(RabbitMqQueues.OrderNotificationRequestQueue,
                    new OrderNotificationServicePayload(requestPayload)
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
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
