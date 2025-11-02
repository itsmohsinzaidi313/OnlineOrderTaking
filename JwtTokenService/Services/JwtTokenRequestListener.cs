using PointofSaleModels.Services;

namespace JwtTokenService.Services
{
    public class JwtTokenRequestListener(ILogger<JwtTokenRequestListener> logger, RabbitMqConnection rabbitConnection, IQueueAction listener) : RabbitMqConsumerService<JwtTokenRequestListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
