using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using System.Security.Claims;

namespace GatewayService
{

    [Authorize]
    public class GatewayHub(Implementation implementation) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = ExtractUserIdFromClaims();
            string connectionId = Context.ConnectionId;
            await implementation.SetUserOnlineAsync(userId, connectionId);
            _ = implementation.SendPendingPayload(userId, connectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetUserOfflineAsync(userId);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task MenuRequest()
        {
            var obj = new GetMenuServicePayload().FillUp(Context);

            await QueuePayload(RabbitMqQueues.MenuRequestQueue, obj);
        }

        public async Task Login(string phoneNumber)
        {
            var obj = new LoginServicePayload
            {
                Customer = new Customer
                {
                    Contact = phoneNumber
                }
            }.FillUp(Context);

            await QueuePayload(RabbitMqQueues.LoginRequestQueue, obj);
        }

        public async Task PlaceOrder(CustomerOrder order)
        {
            var obj = new CreateOrderServicePayload
            {
                Order = order
            }.FillUp(Context);
            await QueuePayload(RabbitMqQueues.OrderRequestQueue, obj);
        }

        private async Task QueuePayload(string queues, ServicePayload payload)
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
