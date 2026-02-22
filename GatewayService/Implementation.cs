using GatewayService.Classes;
using GatewayService.Interfaces;
using GatewayService.Models;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService
{
    public class Implementation(ILogger<Implementation> logger, IHubContext<GatewayHub> hub, StorageManager storage, ConnectionManager connectionManager, IRabbitMqPublisher publisher)
    {
        internal async Task SendPendingPayload(string userId)
        {
            var deserializers = new Dictionary<string, Func<string, ServicePayload?>>()
            {
                { "DataResponse", json => JsonSerializer.Deserialize<DataServicePayload>(json) },
                { "OrderResponse", json => JsonSerializer.Deserialize<OrderServicePayload>(json) }
            };
            while (true)
            {
                var json = await storage.GetPendingAndPop(userId);
                if (json == null) break;

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

        public async Task SendToUser<T>(string svcPayload) where T : ServicePayload
        {
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var clientId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");
            var responseKey = root.GetProperty("ResponseKey").GetString() ?? throw new Exception("ResponseKey not found");

            logger.LogInformation("Gateway: Received {method} message", responseKey);
            var isOnline = await connectionManager.ClientIdExistsAsync(userId);
            if (!isOnline)
            {
                var pendingPayload = new PendingPayload<T>
                {
                    Payload = JsonSerializer.Deserialize<T>(svcPayload)!
                };
                var pendingPayloadJson = JsonSerializer.Serialize(pendingPayload);
                await storage.PushToPending(userId, pendingPayloadJson);
            }
            else
            {
                var payload = JsonSerializer.Deserialize<T>(svcPayload)!;
                await hub.Clients.User(clientId).SendAsync(responseKey, payload);
                return;
            }
        }

        internal async Task QueueRequestPayload<T>(string queues, T payload)
        {
            await publisher.PublishToQueueAsync(queues, payload);
        }
    }
}
