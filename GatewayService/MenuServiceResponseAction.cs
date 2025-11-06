using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

namespace GatewayService
{
    public class MenuServiceResponseAction(ILogger<MenuServiceResponseAction> logger, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuResponseQueue;
        public async Task OnMessage(RabbitMqTransport transport)
        {
            logger.LogInformation("Gateway: Received message");
            var userId = transport.UserId;
            var db = redis.GetDatabase();
            string? connectionId = await db.StringGetAsync($"user:{userId}:connection");
            if (connectionId == null)
            {
                logger.LogInformation("Gateway: User {UserId} is not connected. Storing response in pending queue.", userId);
                await PublishToPending(userId, System.Text.Json.JsonSerializer.Serialize(transport.Payload));
            }
            else
            {
                logger.LogInformation("Gateway: User {UserId} is connected with ConnectionId {ConnId}. Sending response.", userId, connectionId);
                await hub.Clients.User(userId).SendAsync("Response", transport.Payload);
                return;

            }
        }

        private async Task PublishToPending(string userId, string payloadJson)
        {
            var db = redis.GetDatabase();
            await db.ListRightPushAsync($"pending:{userId}", payloadJson);
        }
    }
}
