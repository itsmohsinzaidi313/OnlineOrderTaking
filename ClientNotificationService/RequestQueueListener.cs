using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace ClientNotificationService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IRabbitMqPublisher publisher,
        IRestaurantDbContextFactory restaurantDbContextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ClientNotificationRequestQueue;
        public override async Task OnMessage(string payload)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<ClientNotificationServicePayload>(payload);
            object? response = null;
            try
            {
                using var dbContext = await restaurantDbContextFactory.CreateDbContextByUrlAsync(requestPayload!.DomainName);
                var userIds = await dbContext.UserBranchMappings
                    .Where(x => x.BranchId == requestPayload.CustomerOrder.BranchId)
                    .Select(x => x.UserId)
                    .ToListAsync();

                requestPayload.NewOrderNotificationKeys = [.. userIds.Select(x => $"branch:{x}:*:connection")];
                await publisher.PublishToQueueAsync(RabbitMqQueues.ClientNotificationGatewayResponse, requestPayload);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order notification request for domain {DomainName}", requestPayload?.DomainName);
            }
        }
    }
}
