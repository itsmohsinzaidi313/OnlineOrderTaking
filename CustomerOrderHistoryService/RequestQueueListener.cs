using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CustomerOrderHistoryService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.CustomerOrderHistoryRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<CustomerOrderHistoryServicePayload>(transport);
            object payload = null;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                if (string.IsNullOrEmpty(requestPayload.OrderToken)) throw new Exception("OrderToken missing for userwise orders list");

                var orders = new List<CustomerOrder>();
                await foreach (var order in impl.GetOrdersAsync(connectionString, requestPayload.OrderToken))
                {
                    orders.Add(order);
                }
                payload = new
                {
                    Success = true,
                    Orders = orders
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch customer order history.");
                payload = new
                {
                    Success = false,
                    Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
            }
            var response = new CustomerOrderHistoryServicePayload(requestPayload)
            {
                DataPayload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.CustomerOrderHistoryResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = contextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
