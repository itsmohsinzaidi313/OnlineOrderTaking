using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace GatewayService
{

    [Authorize]
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
            if (claims == null)
                return string.Empty;

            // Try a few possible claim type names. Depending on JWT mapping behaviors the claim
            // may appear as the registered 'sid', as ClaimTypes.Sid, as NameIdentifier or simply 'sid'.
            var userClaim = claims.FirstOrDefault(c =>
                string.Equals(c.Type, JwtRegisteredClaimNames.Sid, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, ClaimTypes.Sid, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, "sid", StringComparison.OrdinalIgnoreCase));

            return userClaim?.Value ?? string.Empty;
        }
    }
}
