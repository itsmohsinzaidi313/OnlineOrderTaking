using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Security.Claims;

namespace GatewayService
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

        [Authorize]
        public async Task SendRequest(string route, string payload)
        {
            await rabbit.InitializeAsync();
            var obj = new RabbitMqTransport
            {
                Route = route,
                Payload = payload,
                ConnectionId = Context.ConnectionId,
                UserId = ExtractUserClaims(),
                BranchId = 0,
                CompanyId = 0
            };

            try
            {
                await rabbit.PublishResponseAsync(obj, RabbitMqQueues.MenuRequestQueue);
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

        private string ExtractUserClaims()
        {
            var claims = Context.User?.Claims;
            var name = claims?.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault()?.Value ?? string.Empty;

            return name ?? string.Empty;
        }
    }
}
