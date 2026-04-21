using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace ExportService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, OrderExportService exportService) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
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
            if (request.OrderNumber == null || request.DomainName == null)
            {
                logger.LogError("Invalid payload: OrderNumber or DomainName is null. Payload: {Payload}", payload);
                return;
            }
            var connectionString = await exportService.GetConnectionString(request.DomainName);
            await exportService.OnMessageHandler(request, connectionString);
        }
    }
}
