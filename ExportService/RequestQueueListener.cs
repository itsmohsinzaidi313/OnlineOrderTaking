using ExportService.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace ExportService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<RestaurantsDbContext> pgContextFactory, IDbContextFactory<SqlServerDbContext> sqlContextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ExportRequestQueue;

        public override Task OnMessage(string payload)
        {
            throw new NotImplementedException();
        }

    }
}
