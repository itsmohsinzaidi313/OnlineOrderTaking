using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;

namespace GatewayService.Hubs
{
    public class GatewayHub(RabbitMqConnection rabbit, ILogger<GatewayHub> logger, IQueueExecution exec) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                await rabbit.InitializeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize RabbitMQ on client connect");
            }
            await base.OnConnectedAsync();
        }

        public async Task SendRequest(string route, string payload)
        {
            await rabbit.InitializeAsync();
            var obj = new RabbitMqTransport
            {
                Route = route,
                Payload = payload,
                ConnectionId = Context.ConnectionId,
                BranchId = 0,
                CompanyId = 0
            };

            try
            {
                await rabbit.PublishResponseAsync(obj, queueName: "services.getmenu.requests.queue");
                logger.LogInformation("Queued message for route {Route} from {ConnId}", route, Context.ConnectionId);
                await Clients.Caller.SendAsync("Ack", new { status = "queued", route, id = Context.ConnectionId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish message to RabbitMQ");
                await Clients.Caller.SendAsync("Ack", new { status = "error", message = ex.Message });
                throw;
            }
        }
    }
}
