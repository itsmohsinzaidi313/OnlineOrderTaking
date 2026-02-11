using Microsoft.Extensions.Logging;
using PointofSaleModels.Services;

namespace SettingsDataService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IQueueAction listener) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
