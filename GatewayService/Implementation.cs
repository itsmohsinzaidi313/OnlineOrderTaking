using GatewayService.Models;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService
{
    public class Implementation
    {
        private readonly ILogger<Implementation> Logger;
        private readonly IHubContext<GatewayHub> Hub;
        private readonly IConnectionMultiplexer Redis;
        private readonly Dictionary<string, Func<string, Task>> Handlers;
        private readonly Dictionary<string, Func<string, ServicePayload?>> Deserializers;
        private readonly IRabbitMqPublisher Publisher;
        private const string PendingKeyPrefix = "pending:";
        public Implementation(ILogger<Implementation> logger, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis, IRabbitMqPublisher publisher)
        {
            Logger = logger;
            Hub = hub;
            Redis = redis;
            Publisher = publisher;
            Handlers = new Dictionary<string, Func<string, Task>>()
            {
                { RabbitMqQueues.LoginResponseQueue, LoginResponseHandler },
                { RabbitMqQueues.MenuResponseQueue, MenuResponseHandler },
                { RabbitMqQueues.OrderResponseQueue, OrderResponseHandler }
            };
            Deserializers = new()
            {
                { "LoginResponse", json => JsonSerializer.Deserialize<LoginServicePayload>(json) },
                { "MenuResponse", json => JsonSerializer.Deserialize<GetMenuServicePayload>(json) },
                { "OrderResponse", json => JsonSerializer.Deserialize<CreateOrderServicePayload>(json) }
            };
        }

        public async Task ExecuteHandler(string queueName, string svcpayload)
        {
            await Handlers[queueName](svcpayload);
        }

        internal async Task SendPendingPayload(string userId)
        {
            var db = Redis.GetDatabase();
            var pendingKey = $"{PendingKeyPrefix}{userId}";

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

                var payload = Deserializers[method](payloadJson);
                if (payload == null) continue;
                await Hub.Clients.User(userId).SendAsync(method, payload);
            }
        }

        private async Task OrderResponseHandler(string payload)
        {
            await SendToUser<CreateOrderServicePayload>("OrderResponse", payload);
        }

        private async Task MenuResponseHandler(string payload)
        {
            await SendToUser<GetMenuServicePayload>("MenuResponse", payload);
        }

        private async Task LoginResponseHandler(string payload)
        {
            await SendToUser<LoginServicePayload>("LoginResponse", payload);
        }

        private void UserConnectedLog(string userId) =>
            Logger.LogInformation("Gateway: User {UserId} is connected. Sending response.", userId);

        private void UserDisconnectedLog(string userId) =>
            Logger.LogInformation("Gateway: User {UserId} is not connected. Storing response in pending queue.", userId);

        private bool UserOnline(string userId)
        {
            var db = Redis.GetDatabase();
            string? connectionId = db.StringGet($"user:{userId}:connection");
            return connectionId != null;
        }

        private async Task SendToUser<T>(string method, string svcPayload) where T : ServicePayload
        {
            Logger.LogInformation("Gateway: Received {method} message", method);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var userId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");

            if (!UserOnline(userId))
            {
                UserDisconnectedLog(userId);
                if (Deserializers[method](svcPayload) is not T deserializedPayload)
                {
                    throw new InvalidOperationException($"Deserialization of payload for method '{method}' returned null.");
                }
                var pendingPayload = new PendingPayload<T>
                {
                    SignalRMethodName = method,
                    Payload = deserializedPayload
                };
                var pendingPayloadJson = JsonSerializer.Serialize(pendingPayload);
                await PublishForPending(userId, pendingPayloadJson);
            }
            else
            {
                UserConnectedLog(userId);
                var payload = Deserializers[method](svcPayload);
                await Hub.Clients.User(userId).SendAsync(method, payload);
                return;
            }
        }

        private async Task PublishForPending(string userId, string payload)
        {
            var db = Redis.GetDatabase();
            await db.ListRightPushAsync($"{PendingKeyPrefix}{userId}", payload);
        }

        internal async Task SetUserOnlineAsync(string userId, string connectionId)
        {
            var db = Redis.GetDatabase();

            // mark online (overwrite previous any)
            await db.StringSetAsync($"user:{userId}:connection", connectionId);
        }

        internal async Task SetUserOfflineAsync(string userId)
        {
            var db = Redis.GetDatabase();
            await db.KeyDeleteAsync($"user:{userId}:connection");
        }

        internal async Task QueueRequestPayload(string queues, ServicePayload payload)
        {
            await Publisher.PublishToQueueAsync(queues, payload);
        }
    }
}
