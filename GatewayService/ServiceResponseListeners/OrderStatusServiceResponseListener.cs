using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderStatusServiceResponseListener(ILogger<OrderStatusServiceResponseListener> logger, RabbitMqConnection rabbitConnection, OrderStatusServiceResponseAction listener) : RabbitMqConsumerService<OrderStatusServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
