using GatewayService.Models;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService
{
    public class Implementation(ILogger<Implementation> logger, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis, IRabbitMqPublisher publisher)
    {
        private const string PendingKeySuffix = ":pending";
        private const string ConnectionKeySuffix = ":connection";

        internal async Task SendPendingPayload(string userId)
        {
            var db = redis.GetDatabase();
            var pendingKey = $"{userId}{PendingKeySuffix}";
            var deserializers = new Dictionary<string, Func<string, ServicePayload?>>()
            {
                { "DataResponse", json => JsonSerializer.Deserialize<DataServicePayload>(json) },
                { "OrderResponse", json => JsonSerializer.Deserialize<OrderServicePayload>(json) }
            };
            while (true)
            {
                var item = await db.ListLeftPopAsync(pendingKey);
                if (!item.HasValue) break;

                var json = item.ToString();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("Payload", out var payloadProp)) continue;
                var payloadJson = payloadProp.GetRawText();

                if (!payloadProp.TryGetProperty("SignalRMethodName", out var methodProp)) continue;

                var method = methodProp.GetString();
                if (string.IsNullOrEmpty(method)) continue;

                var payload = deserializers[method](payloadJson);
                if (payload == null) continue;

                if (!payloadProp.TryGetProperty("ResponseKey", out var responseKeyProp)) continue;
                var responseKey = responseKeyProp.GetString() ?? throw new Exception("ResposneKey not found");

                await hub.Clients.User(userId).SendAsync(responseKey, payload);
            }
        }

        private bool UserOnline(string clientId)
        {
            var db = redis.GetDatabase();
            string? connectionId = db.StringGet($"{clientId}{ConnectionKeySuffix}");
            return connectionId != null;
        }

        public async Task SendToUser<T>(string svcPayload) where T : ServicePayload
        {
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var clientId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");
            var responseKey = root.GetProperty("ResponseKey").GetString() ?? throw new Exception("ResponseKey not found");

            logger.LogInformation("Gateway: Received {responseKey} message", responseKey);
            var payload = JsonSerializer.Deserialize<T>(svcPayload)!;

            if (!UserOnline(clientId))
            {
                var pendingPayload = new PendingPayload<T>
                {
                    Payload = payload!
                };
                var pendingPayloadJson = JsonSerializer.Serialize(pendingPayload);
                await PublishForPending(clientId, pendingPayloadJson);
            }
            else
            {
                logger.LogInformation("Gateway: Sending {responseKey} message to user {clientId}", responseKey, clientId);
                await SendToUser(clientId, responseKey, payload);
            }
        }

        public async Task SendToUser<T>(string clientId, string method, T payload) where T : ServicePayload
        {
            await hub.Clients.User(clientId).SendAsync(method, payload);
        }
        public async Task SendToUsers<T>(List<string> clientIds, string method, T payload) where T : ServicePayload
        {
            await hub.Clients.Users(clientIds).SendAsync(method, payload);
        }

        public async Task SendCustomerOrderToBranches(CustomerOrder svcPayload, List<string> clientIds)
        {
            await hub.Clients.Users(clientIds).SendAsync("NewOrder", svcPayload);
        }

        private async Task PublishForPending(string clientId, string payload)
        {
            var db = redis.GetDatabase();
            await db.ListRightPushAsync($"{clientId}{PendingKeySuffix}", payload);
        }

        internal async Task SetClientOnlineAsync(string clientId, string connectionId)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync($"{clientId}{ConnectionKeySuffix}", connectionId, expiry: TimeSpan.FromHours(4));
        }

        internal async Task SetUserOfflineAsync(string clientId)
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync($"{clientId}{ConnectionKeySuffix}");
        }

        internal async Task QueueRequestPayload<T>(string queues, T payload)
        {
            await publisher.PublishToQueueAsync(queues, payload);
        }
    }
}
