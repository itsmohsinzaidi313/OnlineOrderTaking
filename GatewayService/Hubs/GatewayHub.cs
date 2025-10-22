using System.Text;
using System.Text.Json;
using GatewayService.Services;
using GatewayService.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace GatewayService.Hubs
{
    public class GatewayHub(RabbitMqConnection rabbit, ILogger<GatewayHub> logger) : Hub
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

        // Client sends a request to be forwarded to RabbitMQ
        public async Task SendRequest(string route, string payload)
        {
            await rabbit.InitializeAsync();

            var envelope = new
            {
                route,
                payload,
                connectionId = Context.ConnectionId,
                sentAt = DateTimeOffset.UtcNow
            };

            var json = JsonSerializer.Serialize(envelope);
            var body = Encoding.UTF8.GetBytes(json);

            try
            {
                await rabbit.PublishAsync(body);
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
