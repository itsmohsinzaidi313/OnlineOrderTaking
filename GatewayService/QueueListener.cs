using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;

namespace GatewayService
{
    public class QueueListener(ILogger<QueueListener> logger, IHubContext<GatewayHub> hub) : IQueueExecution
    {
        public async Task OnMessage(RabbitMqTransport transport)
        {
            logger.LogInformation("Gateway: Received message");
            await hub.Clients.Client(transport.ConnectionId)
                .SendAsync("Response", transport.Payload);
        }
    }
}
