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
        private readonly Dictionary<string, Func<ServicePayload, Task>> Handlers;
        private readonly IRabbitMqPublisher Publisher;
        private const string PendingKeyPrefix = "pending:";
        public Implementation(ILogger<Implementation> logger, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis, IRabbitMqPublisher publisher)
        {
            Logger = logger;
            Hub = hub;
            Redis = redis;
            Publisher = publisher;
            Handlers = new Dictionary<string, Func<ServicePayload, Task>>()
            {
                { RabbitMqQueues.LoginResponseQueue, LoginResponseHandler },
                { RabbitMqQueues.MenuResponseQueue, MenuResponseHandler },
                { RabbitMqQueues.OrderResponseQueue, OrderResponseHandler }
            };
        }

        public async Task ExecuteHandler(string queueName, ServicePayload svcpayload)
        {

            Handlers.TryGetValue(queueName, out var handler);
            if (handler != null)
            {
                await handler(svcpayload);
            }
        }

        internal async Task SendPendingPayload(string userId, string connectionId)
        {
            // try pending delivery
            var db = Redis.GetDatabase();
            var pendingKey = $"{PendingKeyPrefix}{userId}";
            long pendingCount = await db.ListLengthAsync(pendingKey);

            if (pendingCount > 0)
            {
                // deliver
                for (int i = 0; i < pendingCount; i++)
                {
                    var item = await db.ListLeftPopAsync(pendingKey);
                    var pendingPayload = JsonSerializer.Deserialize<PendingPayload<ServicePayload>>(item);
                    if (pendingPayload != null)
                    {
                        var method = pendingPayload.SignalRMethodName;
                        if (method == "MenuResponse")
                        {
                            var payload = JsonSerializer.Deserialize<PendingPayload<GetMenuServicePayload>>(item);
                            await Hub.Clients.Client(connectionId).SendAsync(method, payload.Payload);
                        }
                        else if (method == "LoginResponse")
                        {
                            var payload = JsonSerializer.Deserialize<PendingPayload<LoginServicePayload>>(item);
                            await Hub.Clients.Client(connectionId).SendAsync(method, payload.Payload);
                        }
                        else if (method == "OrderResponse")
                        {
                            var payload = JsonSerializer.Deserialize<PendingPayload<CreateOrderServicePayload>>(item);
                            await Hub.Clients.Client(connectionId).SendAsync(method, payload.Payload);
                        }
                    }
                }

                // cleanup
                await db.KeyDeleteAsync(pendingKey);
            }
        }

        private async Task OrderResponseHandler(ServicePayload svcpayload)
        {
            var payload = svcpayload.GetPayload<CreateOrderServicePayload>();
            await SendToUser("OrderResponse", payload);
        }

        private async Task MenuResponseHandler(ServicePayload svcpayload)
        {
            var payload = svcpayload.GetPayload<GetMenuServicePayload>();
            await SendToUser("MenuResponse", payload);
        }

        private async Task LoginResponseHandler(ServicePayload svcpayload)
        {
            var payload = svcpayload.GetPayload<LoginServicePayload>();
            await SendToUser("LoginResponse", payload);
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

        private async Task SendToUser<T>(string method, T payload) where T : ServicePayload
        {
            Logger.LogInformation($"Gateway: Received {method} message");
            var userId = payload.UserId;
            if (!UserOnline(userId))
            {
                UserDisconnectedLog(userId);
                await PublishToPending(method, payload);
            }
            else
            {
                UserConnectedLog(userId);
                await Hub.Clients.User(userId).SendAsync(method, payload);
                return;
            }
        }

        private async Task PublishToPending<T>(string method, T payload) where T : ServicePayload
        {
            var db = Redis.GetDatabase();
            var pendingPayload = new PendingPayload<T>
            {
                SignalRMethodName = method,
                Payload = payload
            };
            var userId = payload.UserId;
            await db.ListRightPushAsync($"{PendingKeyPrefix}{userId}", JsonSerializer.Serialize(pendingPayload));
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
