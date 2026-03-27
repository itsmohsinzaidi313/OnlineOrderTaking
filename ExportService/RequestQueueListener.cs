using ExportService.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace ExportService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<RestaurantsDbContext> pgContextFactory, IDbContextFactory<SqlServerDbContext> sqlContextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ExportRequestQueue;

        public async override Task OnMessage(string payload)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<ExportServicePayload>(payload);
            if (request == null)
            {
                logger.LogError("Failed to deserialize payload: {Payload}", payload);
                return;
            }
        }
    }
}
