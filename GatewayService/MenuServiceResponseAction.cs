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
            if (!string.IsNullOrWhiteSpace(transport.UserId))
            {
                logger.LogInformation("Gateway: Sending response to user {UserId}", transport.UserId);
                await hub.Clients.User(transport.UserId).SendAsync("Response", transport.Payload);
                return;
            }

            logger.LogInformation("Gateway: Sending response to connection {ConnId}", transport.ConnectionId);
            await hub.Clients.Client(transport.ConnectionId)
                .SendAsync("Response", transport.Payload);
        }
    }
}
