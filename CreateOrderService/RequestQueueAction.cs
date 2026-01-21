using Microsoft.Extensions.Logging;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace CreateOrderService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl) : IQueueAction
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
            await impl.SaveOrderAsync(payload.RestaurantId, payload.BranchId, payload.Order!);
        }
    }
}
