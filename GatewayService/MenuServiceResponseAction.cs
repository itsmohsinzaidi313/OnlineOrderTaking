using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService
{
    public class MenuServiceResponseAction(ILogger<MenuServiceResponseAction> logger, IHubContext<GatewayHub> hub) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuResponseQueue;
        public async Task OnMessage(RabbitMqTransport transport)
        {
            logger.LogInformation("Gateway: Received message");
            await hub.Clients.Client(transport.ConnectionId)
                .SendAsync("Response", transport.Payload);
        }
    }
}
