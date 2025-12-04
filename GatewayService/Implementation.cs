using GatewayService.Models;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService
{
    public class Implementation(ILogger<Implementation> logger, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis, IRabbitMqPublisher publisher)
    {
        private const string PendingKeyPrefix = "pending:";

        internal async Task SendPendingPayload(string userId)
        {
            var db = redis.GetDatabase();
            var pendingKey = $"{PendingKeyPrefix}{userId}";
            var deserializers = new Dictionary<string, Func<string, ServicePayload?>>()
            {
                { "LoginResponse", json => JsonSerializer.Deserialize<LoginServicePayload>(json) },
                { "MenuResponse", json => JsonSerializer.Deserialize<GetMenuServicePayload>(json) },
                { "OrderResponse", json => JsonSerializer.Deserialize<CreateOrderServicePayload>(json) }
            };
            while (true)
            {
                var item = await db.ListLeftPopAsync(pendingKey);
                if (!item.HasValue) break;

                var json = item.ToString();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("SignalRMethodName", out var methodProp)) continue;

                var method = methodProp.GetString();
                if (string.IsNullOrEmpty(method)) continue;

                if (!root.TryGetProperty("Payload", out var payloadProp)) continue;
                var payloadJson = payloadProp.GetRawText();

                var payload = deserializers[method](payloadJson);
                if (payload == null) continue;
                await hub.Clients.User(userId).SendAsync(method, payload);
            }
        }

        private void UserConnectedLog(string userId) =>
            logger.LogInformation("Gateway: User {UserId} is connected. Sending response.", userId);

        private void UserDisconnectedLog(string userId) =>
            logger.LogInformation("Gateway: User {UserId} is not connected. Storing response in pending queue.", userId);

        private bool UserOnline(string userId)
        {
            var db = redis.GetDatabase();
            string? connectionId = db.StringGet($"user:{userId}:connection");
            return connectionId != null;
        }

        public async Task SendToUser<T>(string method, string svcPayload) where T : ServicePayload
        {
            logger.LogInformation("Gateway: Received {method} message", method);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var userId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");

            if (!UserOnline(userId))
            {
                UserDisconnectedLog(userId);
                var pendingPayload = new PendingPayload<T>
                {
                    SignalRMethodName = method,
                    Payload = JsonSerializer.Deserialize<T>(svcPayload)!
                };
                var pendingPayloadJson = JsonSerializer.Serialize(pendingPayload);
                await PublishForPending(userId, pendingPayloadJson);
            }
            else
            {
                UserConnectedLog(userId);
                var payload = JsonSerializer.Deserialize<T>(svcPayload)!;
                await hub.Clients.User(userId).SendAsync(method, payload);
                return;
            }
        }

        private async Task PublishForPending(string userId, string payload)
        {
            var db = redis.GetDatabase();
            await db.ListRightPushAsync($"{PendingKeyPrefix}{userId}", payload);
        }

        internal async Task SetUserOnlineAsync(string userId, string connectionId)
        {
            var db = redis.GetDatabase();

            // mark online (overwrite previous any)
            await db.StringSetAsync($"user:{userId}:connection", connectionId);
        }

        internal async Task SetUserOfflineAsync(string userId)
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync($"user:{userId}:connection");
        }

        internal async Task QueueRequestPayload(string queues, ServicePayload payload)
        {
            await publisher.PublishToQueueAsync(queues, payload);
        }
    }
}
