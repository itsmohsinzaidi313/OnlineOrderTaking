using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

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

        public async Task SendRequest(string route, string payload)
        {
            await rabbit.InitializeAsync();
            var obj = new RabbitMqTransport
            {
                Route = route,
                Payload = payload,
                ConnectionId = Context.ConnectionId,
                UserId = GetUserIdFromContext(),
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

        /// <summary>
        /// Request a JWT token for the given userId. Publishes a message to the services.jwt.request-queue
        /// which the JwtTokenService listens to. The payload will contain { userId } and ConnectionId set so
        /// the JwtTokenService can respond back to the Gateway queue.
        /// </summary>
        public async Task RequestJwtToken(string userId)
        {
            await rabbit.InitializeAsync();
            var obj = new RabbitMqTransport
            {
                Route = "jwt.request",
                Payload = new { userId },
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                BranchId = 0,
                CompanyId = 0
            };

            try
            {
                await rabbit.PublishResponseAsync(obj, RabbitMqQueues.JwtRequestQueue);
                logger.LogInformation("Queued JWT request for user {User} from {ConnId}", userId, Context.ConnectionId);
                await Clients.Caller.SendAsync("Ack", new { status = "queued", route = "jwt.request", id = Context.ConnectionId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish JWT request to RabbitMQ");
                await Clients.Caller.SendAsync("Ack", new { status = "error", message = ex.Message });
                throw;
            }
        }

        private string GetUserIdFromContext()
        {
            if (Context.User == null) return string.Empty;

            // Check common claim types that JWT providers use for user id
            var claimTypes = new[] { "userId", "sub" };
            foreach (var ct in claimTypes)
            {
                var claim = Context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, ct, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(claim?.Value)) return claim.Value!;
            }

            return string.Empty;
        }
    }
}
