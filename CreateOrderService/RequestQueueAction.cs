using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueAction(
        ILogger<RequestQueueAction> logger,
        IRabbitMqPublisher publisher,
        Implementation impl,
        IDbContextFactory<Db.RestaurantsContext> contextFactory) : IQueueAction
    {

        public string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public async Task OnMessage(string transport)
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
                var orderNumber = await impl.SaveOrderAsync(connectionString, requestPayload.BranchId, requestPayload.Order!);
                response = new { Success = true, Message = "Order processed successfully", OrderNumber = orderNumber, requestPayload.Order };
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
    }
}
