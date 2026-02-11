using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.OrderHistoryRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                if (!requestPayload.OrderUserId.HasValue) throw new Exception("UserId missing for userwise orders list");

                var orders = new List<CustomerOrder>();
                await foreach (var order in impl.GetOrdersAsync(connectionString, requestPayload.OrderUserId.Value))
                {
                    orders.Add(order);
                }
                var orderStatuses = await impl.GetOrderStatusesAsync(connectionString);
                payload = new { Orders = orders, OrderStatuses = orderStatuses };
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

    enum RequestType
    {
        Menu, DeliveryAndPickup
    }
}
