using GatewayService.Interfaces;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderHistoryServiceResponseAction(Implementation implementation, IConnectionMultiplexer redis) : IOrderHistoryResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderHistoryResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<DataServicePayload>(svcPayload);
        }
    }
}
