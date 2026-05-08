using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderUpdateServiceResponseListener(ILogger<OrderUpdateServiceResponseListener> logger, RabbitMqConnection rabbitConnection, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis) : RabbitMqConsumerService<OrderUpdateServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderUpdateResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            var payload = JsonSerializer.Deserialize<OrderUpdatePayload>(svcPayload);
            var server = redis.GetServer(redis.GetEndPoints().First());
            List<string> keys = [];
            var responseKey = payload?.ResponseKey ?? throw new Exception("ResponseKey not found");
            foreach (var id in payload.BranchUserIds)
            {
                foreach (var key in server.Keys(pattern: $"branch:{id}:*:connection"))
                {
                    var arr = key.ToString().Split(':');
                    var clientId = $"{arr[0]}:{arr[1]}:{arr[2]}";
                    if (string.IsNullOrEmpty(clientId))
                    {
                        continue;
                    }
                    logger.LogInformation($"Sending response to backpanel client '{clientId}' '{responseKey}'");
                    keys.Add(clientId);
                }
            }
            await hub.Clients.Users(keys).SendAsync(responseKey, payload);

            keys.Clear();

            var orderNumber = payload?.OrderToken ?? throw new Exception("OrderNumber not found");
            foreach (var key in server.Keys(pattern: $"order:{orderNumber}:*"))
            {
                var arr = key.ToString().Split(':');
                var clientId = $"{arr[2]}:{arr[3]}:{arr[4]}";
                if (string.IsNullOrEmpty(clientId))
                {
                    continue;
                }
                logger.LogInformation($"Sending response to website client '{clientId}'");
                await hub.Clients.User(clientId).SendAsync(responseKey, payload);
            }
        }
    }
}
