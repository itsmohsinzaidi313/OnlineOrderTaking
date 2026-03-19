using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class ClientNotificationServiceResponseListener(ILogger<ClientNotificationServiceResponseListener> logger, RabbitMqConnection rabbitConnection, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis) : RabbitMqConsumerService<ClientNotificationServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ClientNotificationGatewayResponse;
        public override async Task OnMessage(string svcPayload)
        {
            var payload = JsonSerializer.Deserialize<ClientNotificationServicePayload>(svcPayload);
            if (payload is not null)
            {
                List<string> clients = [];
                var db = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());
                foreach (var key in payload.NewOrderNotificationKeys)
                {
                    clients.AddRange(server.Keys(pattern: key).Select(x => x.ToString().Replace(":connection", "")));
                }
                if (clients.Count > 0)
                    await hub.Clients.Users(clients).SendAsync("NewOrder", payload.CustomerOrder);
            }
        }
    }
}
