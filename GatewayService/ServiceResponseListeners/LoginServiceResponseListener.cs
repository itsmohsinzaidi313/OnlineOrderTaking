using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class LoginServiceResponseListener(ILogger<LoginServiceResponseListener> logger, RabbitMqConnection rabbitConnection, LoginServiceResponseAction listener) : RabbitMqConsumerService<LoginServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
