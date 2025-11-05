using Microsoft.Extensions.Logging;
using PointofSaleModels.Services;

namespace CreateOrderService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, RequestQueueAction listener) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
