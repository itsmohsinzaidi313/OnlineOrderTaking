using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderHistoryRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                if (!requestPayload.OrderUserId.HasValue) throw new Exception("UserId missing for userwise orders list");

                var orders = await impl.GetOrdersAsync(connectionString, requestPayload.OrderUserId.Value).ToListAsync();
                var orderStatuses = await impl.GetOrderStatusesAsync(connectionString);
                var riders = await impl.GetRidersAsync(requestPayload.OrderUserId.Value, connectionString);
                var branches = await impl.GetBranchesAsync(connectionString);
                payload = new { Orders = orders, OrderStatuses = orderStatuses, Riders = riders, Branches = branches };
                success = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch data.");
                success = false;
                payload = new
                {
                    Success = false,
                    Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
            }
            var response = new DataServicePayload(requestPayload)
            {
                Success = success,
                DataPayload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderHistoryResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = contextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
    }
}
