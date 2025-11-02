using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json;

namespace GatewayService
{
    public class JwtServiceResponseAction(IHubContext<GatewayHub> hub) : IQueueAction
    {
        string IQueueAction.QueueName() => RabbitMqQueues.JwtRequestQueue;

        async Task IQueueAction.OnMessage(RabbitMqTransport transport)
        {
            await hub.Clients.Client(transport.ConnectionId).SendAsync("JwtTokenResponse", transport.Payload);
        }
    }
}
