using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;

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
            // Prefer authenticated principal
            if (Context.User?.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                // Check several common claim types that JWT providers use for user id
                var claimTypes = new[] { ClaimTypes.NameIdentifier, "nameid", "userId", "userid", "user_id", "sub" };
                foreach (var ct in claimTypes)
                {
                    var claim = Context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, ct, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(claim?.Value)) return claim.Value!;
                }
            }

            // If there's no authenticated principal (common with SignalR when token is passed via query string),
            // attempt to read the access_token from the HTTP context and parse the JWT without validating signature
            var http = Context.GetHttpContext();
            if (http == null) return string.Empty;

            string? token = null;

            // 1) Query string parameter used by SignalR clients: access_token
            if (http.Request.Query.TryGetValue("access_token", out StringValues v) && !StringValues.IsNullOrEmpty(v))
            {
                token = v.ToString();
            }

            // 2) Authorization header as fallback
            if (string.IsNullOrWhiteSpace(token) && http.Request.Headers.TryGetValue("Authorization", out var authHeader) && !StringValues.IsNullOrEmpty(authHeader))
            {
                var header = authHeader.ToString();
                if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = header.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(token)) return string.Empty;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var claimCandidates = new[] { ClaimTypes.NameIdentifier, "nameid", "userId", "userid", "user_id", "sub" };
                foreach (var ct in claimCandidates)
                {
                    var claim = jwt.Claims.FirstOrDefault(c => string.Equals(c.Type, ct, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(claim?.Value)) return claim.Value!;
                }

                // As a last resort try the standard 'sub' registered name
                var sub = jwt.Claims.FirstOrDefault(c => string.Equals(c.Type, JwtRegisteredClaimNames.Sub, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(sub?.Value)) return sub.Value!;
            }
            catch
            {
                // If the token is malformed, swallow and return empty (caller will treat as unauthenticated)
            }

            return string.Empty;
        }
    }
}
