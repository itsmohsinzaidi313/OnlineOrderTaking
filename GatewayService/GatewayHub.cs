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

        public async Task MenuRequest(string domainName, int branchId, string responseKey)
        {
            var code = await storage.PublishForService(new DataServicePayload
            {
                DomainName = domainName,
                BranchId = branchId.ToString(),
                ResponseKey = responseKey,
                SignalRMethod = "MenuRequest"
            }.FillContext(Context));

            if (!response.IsNull)
            {
                var payload = JsonSerializer.Deserialize<DataServicePayload>(response.ToString());
                await Clients.Caller.SendAsync("Ack", new { status = "cached" });
                await Clients.Caller.SendAsync(responseKey, payload);
                return;
            }
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = string.Empty,
                BranchId = branchId,
                ResponseKey = responseKey,
                SignalRMethodName = "MenuRequest"
            }.FillContext(Context);

            await QueuePayload(RabbitMqQueues.MenuRequestQueue, obj);
        }

        public async Task DeliveryAndPickupRequest(string domainName, int branchId, string responseKey)
        {
            var db = redis.GetDatabase();
            var response = await db.StringGetAsync($"{domainName}:{branchId}:dandp");
            if (!response.IsNull)
            {
                var payload = JsonSerializer.Deserialize<DataServicePayload>(response.ToString());
                await Clients.Caller.SendAsync("Ack", new { status = "cached" });
                await Clients.Caller.SendAsync(responseKey, payload);
                return;
            }
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = string.Empty,
                BranchId = branchId,
                ResponseKey = responseKey,
                SignalRMethodName = "DeliveryAndPickupRequest"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.SettingRequestQueue, obj);
        }

        public async Task OrderHistoryRequest(string domainName, int userId, string? orderToken, string responseKey)
        {
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = "Orders",
                ResponseKey = responseKey,
                SignalRMethodName = "OrderHistoryRequest"
            }.FillContext(Context);
            obj.OrderUserId = userId;
            obj.OrderToken = orderToken;
            await QueuePayload(RabbitMqQueues.OrderHistoryRequestQueue, obj);
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
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = requestType,
                BranchId = branchId,
                ResponseKey = responseKey,
                SignalRMethodName = "DataResponse"
            }.FillContext(Context);

            if (requestType == "Orders")
            {
                obj.OrderUserId = branchId;
            }
            await QueuePayload(RabbitMqQueues.DataRequestQueue, obj);
        }

        public async Task PlaceOrder(CustomerOrder order, string responseKey)
        {
            var obj = new OrderServicePayload
            {
                Order = order,
                BranchId = order.BranchId,
                DomainName = order.Domain,
                ResponseKey = responseKey,
                SignalRMethodName = "PlaceOrder"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.OrderRequestQueue, obj);
        }

        public async Task OrderStatus(string domainName, int branchId, string orderNumber, int? orderStatusId, int? branchTransferId, int? riderId, int? deliveryTime, string responseKey)
        {
            var obj = new OrderUpdatePayload
            {
                DomainName = domainName,
                BranchId = branchId,
                OrderToken = orderNumber,
                ResponseKey = responseKey,
                BranchTransferId = branchTransferId,
                OrderStatusId = orderStatusId,
                DeliveryTime = deliveryTime,
                RiderId = riderId,
                SignalRMethodName = "OrderStatus"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.OrderUpdateRequestQueue, obj);
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
