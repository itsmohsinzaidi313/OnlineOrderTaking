using GatewayService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace GatewayService
{

    [Authorize]
    public class GatewayHub(Implementation implementation, StorageManager storage, IConnectionManager connectionManager) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = ExtractUserIdFromClaims();
            await connectionManager.AddClientIdAsync(userId, Context.ConnectionId);
            await implementation.SendPendingPayload(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string userId = ExtractUserIdFromClaims();
            await connectionManager.RemoveClientIdAsync(userId);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task DataRequest(string domainName, string requestType, int branchId, string responseKey)
        {
            var redisKey = requestType switch
            {
                "Menu" => "Menu",
                "DeliveryAndPickup" => "DAndP",
                _ => string.Empty
            };

            var servicePayload = new DataServicePayload
            {
                DomainName = domainName,
                RequestType = requestType,
                BranchId = branchId.ToString(),
                ResponseKey = responseKey,
                SignalRMethod = "DataRequest",
            }.FillContext(Context);

            if (!string.IsNullOrEmpty(redisKey))
            {
                var response = await storage.GetCachedStringAsync($"{domainName}:{branchId}:{redisKey}");
                if (!string.IsNullOrEmpty(response))
                {
                    var payload = JsonSerializer.Deserialize<DataServicePayload>(response.ToString());
                    await Clients.Caller.SendAsync("Ack", new { status = "cached" });
                    await Clients.Caller.SendAsync(responseKey, payload);
                    return;
                }
            }

            await QueuePayload(RabbitMqQueues.DataRequestQueue, servicePayload);
        }

        public async Task PlaceOrder(CustomerOrder order, string responseKey)
        {
            var obj = new OrderServicePayload
            {
                Order = order,
                BranchId = order.BranchId,
                DomainName = order.Domain
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.OrderRequestQueue, obj);
        }

        private async Task QueuePayload<T>(string queues, T payload)
        {
            await implementation.QueueRequestPayload(queues, payload);
            await Clients.Caller.SendAsync("Ack", new { status = "queued" });
        }

        internal string ExtractUserIdFromClaims()
        {
            return Context.User?.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        }
    }
}
