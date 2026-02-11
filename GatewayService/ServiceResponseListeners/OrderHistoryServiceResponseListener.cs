using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderHistoryServiceResponseListener(ILogger<OrderHistoryServiceResponseListener> logger, RabbitMqConnection rabbitConnection, OrderHistoryServiceResponseAction listener) : RabbitMqConsumerService<OrderHistoryServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
