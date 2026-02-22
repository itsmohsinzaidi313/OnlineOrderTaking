using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderStatusServiceResponseListener(ILogger<OrderStatusServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation) : RabbitMqConsumerService<OrderStatusServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderStatusResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderStatusPayload>(svcPayload);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
