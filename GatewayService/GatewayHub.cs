using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace GatewayService
{

    [Authorize]
    public class GatewayHub(Implementation implementation, IConnectionMultiplexer redis) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetUserOnlineAsync(userId, Context.ConnectionId);
            await implementation.SendPendingPayload(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetUserOfflineAsync(userId);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task ImportRequest(int restaurantId)
        {
            await QueuePayload(RabbitMqQueues.ImportRequestQueue, new ImportServicePayload
            {
                RestaurantId = restaurantId
            }.FillContext(Context));
        }

        public async Task DataRequest(string domainName, string requestType, int branchId, string responseKey)
        {
            var db = redis.GetDatabase();
            var redisKey = requestType switch
            {
                "Menu" => "Menu",
                "DeliveryAndPickup" => "DAndP",
                _ => string.Empty
            };
            if (!string.IsNullOrEmpty(redisKey))
            {
                var response = await db.StringGetAsync($"{domainName}:{branchId}:{redisKey}");
                if (!response.IsNull)
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
                ResponseKey = responseKey
            }.FillContext(Context);

            await QueuePayload(RabbitMqQueues.DataRequestQueue, obj);
        }

        public async Task Login(string phoneNumber)
        {
            var obj = new LoginServicePayload
            {
                Customer = new Customer
                {
                    Contact = phoneNumber
                }
            }.FillContext(Context);

            await QueuePayload(RabbitMqQueues.LoginRequestQueue, obj);
        }

        public async Task PlaceOrder(CustomerOrder order)
        {
            var obj = new OrderServicePayload
            {
                Order = order
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
