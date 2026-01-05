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

        public async Task DataRequest(string domainName, string requestType)
        {
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = requestType
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
