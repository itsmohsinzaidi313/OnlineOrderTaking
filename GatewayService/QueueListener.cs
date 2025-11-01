using GatewayService.Hubs;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;

namespace GatewayService
{
    public class QueueListener(IHubContext<GatewayHub> hub) : IQueueExecution
    {
        public async Task OnMessage(RabbitMqTransport transport)
        {
            await hub.Clients.Client(transport.ConnectionId)
                .SendAsync("Response", transport.Payload);
        }
    }
}
