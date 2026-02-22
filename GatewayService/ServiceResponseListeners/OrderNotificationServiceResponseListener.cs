using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderNotificationServiceResponseListener(ILogger<OrderNotificationServiceResponseListener> logger, RabbitMqConnection rabbitConnection, IHubContext<GatewayHub> hub, IConnectionMultiplexer redis) : RabbitMqConsumerService<OrderNotificationServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderNotificationGatewayResponse;
        public override async Task OnMessage(string svcPayload)
        {
            var payload = JsonSerializer.Deserialize<OrderNotificationServicePayload>(svcPayload);
            if (payload is not null)
            {
                List<string> clients = [];
                var db = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());
                foreach (var key in payload.NotificationKeys)
                {
                    clients.AddRange(server.Keys(pattern: key).Select(x => x.ToString().Replace(":connection", "")));
                }
                await hub.Clients.Users(clients).SendAsync("NewOrder", payload.CustomerOrder);
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
