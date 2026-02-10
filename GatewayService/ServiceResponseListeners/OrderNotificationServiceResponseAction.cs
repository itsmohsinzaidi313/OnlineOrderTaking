using GatewayService.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderNotificationServiceResponseAction(IHubContext<GatewayHub> hub, IConnectionMultiplexer redis) : IOrderNotificationResponseAction
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = null,
            DictionaryKeyPolicy = null,
        };
        public string QueueName() => RabbitMqQueues.OrderNotificationGatewayResponse;
        public async Task OnMessage(string svcPayload)
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
                var p = JsonSerializer.Serialize(payload.CustomerOrder, SerializerOptions);
                var pp = JsonSerializer.Deserialize<CustomerOrder>(p, SerializerOptions);
                await hub.Clients.Users(clients).SendAsync("NewOrder", pp);
            }
        }
    }
}
