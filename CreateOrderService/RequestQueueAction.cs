using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, Db.RestaurantsContext context) : IQueueAction
    {

        public string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public async Task OnMessage(string transport)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<OrderServicePayload>(transport);
            if (payload == null)
            {
                logger.LogWarning("Invalid or missing order payload for company {CompanyId}, branch {BranchId}", payload.RestaurantId, payload.BranchId);
                throw new InvalidOperationException("Invalid order payload");
            }
            var connectionString = await GetConnectionString(payload.DomainName);
            await impl.SaveOrderAsync(connectionString, payload.BranchId, payload.Order!);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
    }
}
