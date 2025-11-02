
using PointofSaleModels.Services;

namespace GatewayService
{
    public class JwtServiceResponseListener(ILogger<JwtServiceResponseListener> logger, RabbitMqConnection rabbitConnection, JwtServiceResponseAction listener) : RabbitMqConsumerService<JwtServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
