using GatewayService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Protos;
using PointofSaleModels.ServicePayloads;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static PointofSaleModels.Protos.PushNotificationService;
using static PointofSaleModels.Protos.OrderHistoryService;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("")]
    public class ApiController(IOptions<JwtSettings> jwtOptions, ILogger<ApiController> logger, IConnectionMultiplexer redis, PushNotificationServiceClient pushNotificationClient, OrderHistoryServiceClient orderHistoryClient) : ControllerBase
    {
        private readonly JwtSettings _jwt = jwtOptions.Value;
        [HttpGet("myorder")]
        public async Task<IActionResult> GetMyOrder([FromQuery] string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return BadRequest(new { error = "Order number is required." });
            var host = HttpContext.Request.Host.Value;
            var orderhistoryRequest = new OrderHistoryRequest
            {
                OrderToken = orderNumber
            };
            var orderHistoryResponse = await orderHistoryClient.GetOrderHistoryAsync(orderhistoryRequest);
            if (orderHistoryResponse.Success == false)
            {
                return Ok(orderHistoryResponse);
            }
            var customerOrders = orderHistoryResponse.OrdersPayload.Select(json => System.Text.Json.JsonSerializer.Deserialize<CustomerOrder>(json)).ToList();
            return Ok(customerOrders);
        }

        [AllowAnonymous]
        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeAsync([FromBody] PushSubscriptionDto dto)
        {
            var request = new PushNotificationSubscriptionRequest
            {
                ClientId = dto.ClientId,
                Endpoint = dto.Endpoint,
                P256Dh = dto.P256DH,
                Auth = dto.Auth
            };
            var response = await pushNotificationClient.SubscribeAsync(request);

            if (response.Success)
                return Ok();
            else
                return BadRequest(response.Message);
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([FromBody] string clientId)
        {
            var request = new PushNotificationUnsubscribeRequest
            {
                ClientId = clientId
            };
            var response = await pushNotificationClient.UnsubscribeAsync(request);

            if (response.Success)
                return Ok();
            else
                return BadRequest(response.Message);
        }

        [HttpPost("notify")]
        public async Task<IActionResult> NotifyAsync(NotifyRequest request)
        {
            var obj = new PushNotificationNotifyRequest
            {
                ClientId = request.ClientId,
                Title = request.Title,
                Message = request.Message
            };
            var response = await pushNotificationClient.NotifyAsync(obj);
            if (response.Success)
                return Ok();
            else
                return BadRequest(response.Message);
        }

        [HttpGet("clear")]
        public async Task<IActionResult> ClearCacheAsync([FromQuery] string domain)
        {
            var db = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints().First());
            int menuKeys = 0, dAndPKeys = 0, pendingKeys = 0, subscriptions = 0;
            foreach (var key in server.Keys(pattern: $"{domain}:*:Menu"))
            {
                await db.KeyDeleteAsync(key);
                menuKeys++;
            }

            foreach (var key in server.Keys(pattern: $"{domain}:*:menu"))
            {
                await db.KeyDeleteAsync(key);
                menuKeys++;
            }

            foreach (var key in server.Keys(pattern: $"{domain}:*:DAndP"))
            {
                await db.KeyDeleteAsync(key);
                dAndPKeys++;
            }

            foreach (var key in server.Keys(pattern: $"{domain}:*:dandp"))
            {
                await db.KeyDeleteAsync(key);
                dAndPKeys++;
            }

            foreach (var key in server.Keys(pattern: "*:pending"))
            {
                await db.KeyDeleteAsync(key);
                pendingKeys++;
            }
            foreach (var key in server.Keys(pattern: "subscription:*"))
            {
                await db.KeyDeleteAsync(key);
                subscriptions++;
            }
            return Ok(new { Menu = menuKeys, DAndP = dAndPKeys, Pending = pendingKeys, Subscriptions = subscriptions });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("Gateway Service is healthy.");
        }

        [HttpGet("import/{companyId:int}")]
        public async Task<IActionResult> Import(int companyId, [FromQuery] bool checkhealth = true, [FromQuery] bool checkOrders = true)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = new Uri("http://importservice:8080"),
            };

            if (checkhealth)
            {
                var healthResponse = await httpClient.GetAsync("health");
                if (!healthResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)healthResponse.StatusCode, "Import service is not healthy.");
                }
            }

            var response = await httpClient.GetAsync($"import/{companyId}?checkOrders={checkOrders}");

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
        public class NotifyRequest
        {
            public string ClientId { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
