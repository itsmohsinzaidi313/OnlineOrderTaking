using Microsoft.Extensions.Logging;
using PointofSaleModels.Application;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.DatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl) : IQueueAction
    {

        public string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public async Task OnMessage(RabbitMqTransport transport)
        {
            var order = transport.GetPayload<CustomerOrder>();
            if (order == null)
            {
                logger.LogWarning("Invalid or missing order payload for company {CompanyId}, branch {BranchId}", transport.CompanyId, transport.BranchId);
                throw new InvalidOperationException("Invalid order payload");
            }
            await impl.SaveOrderAsync(int.Parse(transport.CompanyId), int.Parse(transport.BranchId), order);
        }
    }
}
