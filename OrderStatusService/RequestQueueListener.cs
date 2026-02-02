using PointofSaleModels.Services;

namespace OrderStatusService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IQueueAction listener) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
