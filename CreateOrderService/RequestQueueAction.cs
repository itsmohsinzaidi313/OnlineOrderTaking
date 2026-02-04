using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, IRabbitMqPublisher publisher, Implementation impl, Db.RestaurantsContext context) : IQueueAction
    {

        public string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public async Task OnMessage(string transport)
        {
            object? response = null;
            try
            {
                var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderServicePayload>(transport);
                if (requestPayload == null)
                {
                    logger.LogWarning("Invalid or missing order payload for company {CompanyId}, branch {BranchId}", requestPayload?.RestaurantId, requestPayload?.BranchId);
                    throw new InvalidOperationException("Invalid order payload");
                }
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                var orderNumber = await impl.SaveOrderAsync(connectionString, requestPayload.BranchId, requestPayload.Order!);
                response = new { Success = true , Message = "Order processed successfully", OrderNumber = orderNumber };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, ex.Message };
            }

            await publisher.PublishToQueueAsync(RabbitMqQueues.DataResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
    }
}
