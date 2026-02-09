using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("")]
    public class ApiController(IOptions<JwtSettings> jwtOptions, ILogger<ApiController> logger, IConnectionMultiplexer redis, Implementation implementation, ApiDataRequestCoordinator apiResponses) : ControllerBase
    {
        private readonly JwtSettings _jwt = jwtOptions.Value;
        private readonly Implementation _implementation = implementation;
        private readonly ApiDataRequestCoordinator _apiResponses = apiResponses;
        [HttpGet("clear")]
        public async Task<IActionResult> ClearCacheAsync([FromQuery] string domain)
        {
            var db = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints().First());
            int menuKeys = 0, dAndPKeys = 0, pendingKeys = 0;
            foreach (var key in server.Keys(pattern: $"{domain}:*:Menu"))
            {
                await db.KeyDeleteAsync(key);
                menuKeys++;
            }

            foreach (var key in server.Keys(pattern: $"{domain}:*:DAndP"))
            {
                await db.KeyDeleteAsync(key);
                dAndPKeys++;
            }

            foreach (var key in server.Keys(pattern: "*:pending"))
            {
                await db.KeyDeleteAsync(key);
                pendingKeys++;
            }
            return Ok(new { Menu = menuKeys, DAndP = dAndPKeys, Pending = pendingKeys });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("Gateway Service is healthy.");
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] string domain, [FromQuery] int branchId, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid().ToString();
            var payload = new DataServicePayload
            {
                CorrelationId = correlationId,
                DomainName = domain,
                BranchId = branchId,
                DataRequestType = "Orders",
                ResponseKey = "OrdersResponse",
                SignalRMethodName = "DataResponse",
                UserId = $"api-{correlationId}"
            };

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

            var waitTask = _apiResponses.WaitForResponseAsync(correlationId, timeoutCts.Token);
            await _implementation.QueueRequestPayload(RabbitMqQueues.DataRequestQueue, payload);

            string responseJson;
            try
            {
                responseJson = await waitTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    return StatusCode(504, "Timed out waiting for data service response.");
                }

                return StatusCode(499, "Request was cancelled.");
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;
            var success = root.TryGetProperty("Success", out var successProp) && successProp.GetBoolean();
            if (!success)
            {
                var errorPayload = root.TryGetProperty("DataPayload", out var err) ? err.GetRawText() : "Request failed.";
                return BadRequest(new { error = errorPayload });
            }

            if (!root.TryGetProperty("DataPayload", out var dataPayload))
            {
                return StatusCode(500, "Missing data payload in response.");
            }

            var orders = JsonSerializer.Deserialize<List<CustomerOrder>>(dataPayload.GetRawText()) ?? new List<CustomerOrder>();
            return Ok(orders);
        }

        [HttpGet("import/{companyId:int}")]
        public async Task<IActionResult> Import(int companyId)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = new Uri("http://importservice:8080")
            };

            var response = await httpClient.GetAsync("health");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Import service is not healthy.");
            }

            response = await httpClient.GetAsync($"import/{companyId}");

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return StatusCode((int)response.StatusCode, $"Import service encountered an internal error for companyId: {companyId}.\n{response.RequestMessage}");
            }

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Import service failed for companyId: {companyId}");
            }

            return Ok($"Import completed successfully for companyId: {companyId}");
        }

        [HttpPost("generate-token")]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            var bad = ValidateJwtOrBad();
            if (bad != null) return bad;
            var userId = request.UserId;
            var token = CreateTokenForUser(userId, "1165", "0");
            return Ok(new { token, userId });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenRequest? body)
        {
            var bad = ValidateJwtOrBad();
            if (bad != null) return bad;

            var token = ExtractIncomingToken(body?.Token);
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { error = "No token supplied. Provide a token in Authorization header or JSON body { token: '...' }." });

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwt.Audience,
                // Allow expired tokens for refresh by skipping lifetime validation here
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out _);
                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { error = "Token does not contain a subject (user id)." });

                var newToken = CreateTokenForUser(userId, "1165", "0");
                return Ok(new { token = newToken, userId });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing token");
                return StatusCode(500);
            }
        }

        private string? ExtractIncomingToken(string? bodyToken)
        {
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var header = authHeader.ToString();
                if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return header.Substring("Bearer ".Length).Trim();
            }
            return string.IsNullOrWhiteSpace(bodyToken) ? null : bodyToken;
        }

        private string CreateTokenForUser(string userId, string companyId, string branchId)
        {
            var claims = new List<Claim>
            {
                new("cid", companyId),
                new("bid", branchId),
                new(ClaimTypes.NameIdentifier, userId),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private BadRequestObjectResult? ValidateJwtOrBad()
        {
            if (string.IsNullOrWhiteSpace(_jwt.Key) || string.IsNullOrWhiteSpace(_jwt.Issuer) || string.IsNullOrWhiteSpace(_jwt.Audience) || _jwt.ExpireMinutes <= 0)
            {
                return BadRequest(new { error = "Jwt settings are not properly configured. Please set Jwt:Key, Jwt:Issuer, Jwt:Audience and Jwt:ExpireMinutes." });
            }
            return null;
        }

        public record TokenRequest(string? Token);
        public record LoginRequest(string Username, string Password, string UserId);
    }
}
