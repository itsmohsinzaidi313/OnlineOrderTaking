using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseListener(ILogger<CreateOrderServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation) : RabbitMqConsumerService<CreateOrderServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderServicePayload>(svcPayload);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
