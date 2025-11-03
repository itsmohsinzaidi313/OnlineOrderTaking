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
            // If the transport contains a UserId, send to that user (all connections for the user).
            if (!string.IsNullOrWhiteSpace(transport.UserId))
            {
                await hub.Clients.User(transport.UserId).SendAsync("Response", transport.Payload);
                return;
            }

            // Fallback to the original behavior (send to a single connection id)
            await hub.Clients.Client(transport.ConnectionId)
                .SendAsync("Response", transport.Payload);
        }
    }
}
