using PointofSaleModels.Services;

namespace GatewayService
{
    public class MenuServiceResponseListener(ILogger<MenuServiceResponseListener> logger, RabbitMqConnection rabbitConnection, MenuServiceResponseAction listener) : RabbitMqConsumerService<MenuServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
