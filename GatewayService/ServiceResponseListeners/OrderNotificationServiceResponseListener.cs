using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderNotificationServiceResponseListener(ILogger<OrderNotificationServiceResponseListener> logger, RabbitMqConnection rabbitConnection, OrderNotificationServiceResponseAction listener) : RabbitMqConsumerService<OrderNotificationServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
