using GatewayService.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderNotificationServiceResponseAction(IHubContext<GatewayHub> hub) : IOrderNotificationResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderNotificationResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            var payload = JsonSerializer.Deserialize<OrderNotificationServicePayload>(svcPayload);
            if (payload is not null)
            {
                await hub.Clients.Users(payload.NotificationKeys).SendAsync("NewOrder", payload.CustomerOrder);
            }
        }
    }
}
