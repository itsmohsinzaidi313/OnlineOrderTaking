using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GatewayService
{

    [Authorize]
    public class GatewayHub(IRabbitMqPublisher publisher, ILogger<GatewayHub> logger) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendRequest(string route, string payload)
        {
            var obj = new RabbitMqTransport
            {
                Route = route,
                Payload = payload,
                ConnectionId = Context.ConnectionId,
                UserId = ExtractUserClaims(),
                BranchId = ExtractBranchIdClaims(),
                CompanyId = ExtractCompanyIdClaims()
            };

            try
            {
                await publisher.PublishToQueueAsync(RabbitMqQueues.MenuRequestQueue, obj);
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
            return Context.User?.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        }

        private string ExtractCompanyIdClaims()
        {
            return Context.User?.Claims.FirstOrDefault(x => string.Equals(x.Type, "cid", StringComparison.OrdinalIgnoreCase))?.Value ?? "0";
        }

        private string ExtractBranchIdClaims()
        {
            return Context.User?.Claims.FirstOrDefault(x => string.Equals(x.Type, "bid", StringComparison.OrdinalIgnoreCase))?.Value ?? "0";
        }
    }
}
