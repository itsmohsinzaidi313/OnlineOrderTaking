using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseListener(ILogger<CreateOrderServiceResponseListener> logger, RabbitMqConnection rabbitConnection, CreateOrderServiceResponseAction listener) : RabbitMqConsumerService<CreateOrderServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
