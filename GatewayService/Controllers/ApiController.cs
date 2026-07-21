using GatewayService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Integrations;
using PointofSaleModels.Protos;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static PointofSaleModels.Protos.CreateOrderService;
using static PointofSaleModels.Protos.FpUploadMenuService;
using static PointofSaleModels.Protos.GeneralSeoDataService;
using static PointofSaleModels.Protos.OrderHistoryService;
using static PointofSaleModels.Protos.PushNotificationService;
using App = PointofSaleModels.Application;


namespace GatewayService.Controllers
{
    [ApiController]
    [Route("")]
    public class ApiController(IOptions<JwtSettings> jwtOptions, ILogger<ApiController> logger, IConnectionMultiplexer redis, PushNotificationServiceClient pushNotificationClient, OrderHistoryServiceClient orderHistoryClient, GeneralSeoDataServiceClient seoDataClient, CreateOrderServiceClient createOrderClient, FpUploadMenuServiceClient fpUploadMenuServiceClient) : ControllerBase
    {
        private readonly JwtSettings _jwt = jwtOptions.Value;

        [AllowAnonymous]
        [HttpPost("PosIntegration/{token}/{order}/{remoteId}")]
        public async Task<IActionResult> FoodpandaIntegration(string token, string order, string remoteId, [FromBody] object payloadModel)
        {
            //var payload = new IntegrationServicePayload<FoodPandaPayloadModel>
            //{
            //    Token = token,
            //    Order = order,
            //    RemoteId = remoteId,
            //    OrderPayload = payloadModel
            //};
            //await impl.QueueRequestPayload(RabbitMqQueues.FoodpandaIntegrationRequestQueue, payload);
            logger.LogInformation($"Received FoodpandaIntegration request: token={token}, order={order}, remoteId={remoteId}, payload={payloadModel}");
            var node = JsonNode.Parse(payloadModel.ToString());
            var callbackUrls = node?["callbackUrls"]?.AsObject();
            var orderAcceptedUrl = callbackUrls?["orderAcceptedUrl"]?.GetValue<string>();
            var accessToken = await RequestAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return StatusCode(500, new { error = "Failed to obtain access token." });
            }
            if (string.IsNullOrEmpty(orderAcceptedUrl))
            {
                return BadRequest(new { error = "orderAcceptedUrl not found in payload." });
            }
            await OrderAcceptedStatus(accessToken, order, orderAcceptedUrl);
            return Ok();
        }
        static async Task<string?> RequestAccessTokenAsync()
        {
            const string baseUrl = "https://integration-middleware.as.restaurant-partners.com";
            const string loginPath = "/v2/login";
            const string username = "as-plugin-y-generation-systems-005";
            const string password = "KQ1D8Wcm0M";
            const string secret = "SnyteunCeerhicJofI";

            var apiUrl = $"{baseUrl.TrimEnd('/')}/{loginPath.TrimStart('/')}";
            var form = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["grant_type"] = "client_credentials"
            };

            if (!string.IsNullOrWhiteSpace(secret))
            {
                form["secret"] = secret;
            }

            using var content = new FormUrlEncodedContent(form);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginResponse = await client.PostAsync(apiUrl, content);
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            Console.WriteLine($"POST {apiUrl} -> {(int)loginResponse.StatusCode} {loginResponse.ReasonPhrase}");
            if (!string.IsNullOrWhiteSpace(loginContent)) Console.WriteLine(loginContent);

            if (!loginResponse.IsSuccessStatusCode)
            {
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(loginContent);
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                return accessTokenElement.GetString();
            }

            return null;
        }
        static async Task OrderAcceptedStatus(string accessToken, string orderCode, string url)
        {
            var content = JsonContent.Create(new
            {
                acceptanceTime = DateTime.Now,
                remoteOrderId = orderCode,
                status = "order_accepted"
            });
            using var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = content,
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST {client.BaseAddress}{request.RequestUri} -> {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        [HttpGet("UpdateFoodpandaMenu")]
        public async Task<IActionResult> UpdateFoodpandaMenu([FromQuery] int id)
        {
            var response = await fpUploadMenuServiceClient.UploadMenuAsync(new FpUploadMenuRequest { Id = id }, cancellationToken: HttpContext.RequestAborted);
            if (response.Success)
                return Ok(response);
            else
                return Problem(response.Message);
        }

        [HttpGet("SEO")]
        public async Task<IActionResult> GetSeoData([FromQuery] string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return BadRequest(new { error = "Domain is required." });
            var list = await seoDataClient.GetSeoDataAsync(new Domain { DomainName = domain }, cancellationToken: HttpContext.RequestAborted);
            return Ok(list);
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder()
        {
            var request = await GetPlaceOrderRequest();
            var response = await createOrderClient.PlaceOrderAsync(request, cancellationToken: HttpContext.RequestAborted);
            if (response.Success)
            {
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
                }
            }
            else
                return BadRequest(response.Message);
        }
        private async Task<PlaceOrderRequest> GetPlaceOrderRequest()
        {
            HttpContext.Request.EnableBuffering();
            HttpContext.Request.Body.Position = 0;
            using var reader = new StreamReader(HttpContext.Request.Body);
            var body = await reader.ReadToEndAsync();
            return new PlaceOrderRequest { OrderJson = body };
        }

        [HttpPost("PlaceOrderLegacy")]
        public async Task<IActionResult> PlaceOrderLegacy()
        {
            var request = await GetPlaceOrderRequest();
            var response = await createOrderClient.PlaceOrderLegacyAsync(request, cancellationToken: HttpContext.RequestAborted);
            var responseBody = JsonSerializer.Deserialize<object>(response.ResponseJson);
            if (response.Success)
            {
                if (response.Success)
                {
                    return Ok(responseBody);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
                }
            }
            else
                return BadRequest(response.Message);
        }

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
            var orderHistoryResponse = await orderHistoryClient.GetOrderHistoryAsync(orderhistoryRequest, cancellationToken: HttpContext.RequestAborted);
            if (orderHistoryResponse.Success == false)
            {
                return Ok(orderHistoryResponse);
            }
            var customerOrders = orderHistoryResponse.OrdersPayload.Select(json => System.Text.Json.JsonSerializer.Deserialize<App.CustomerOrder>(json)).ToList();
            return Ok(customerOrders);
        }

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
            int menuKeys = 0, dAndPKeys = 0, pendingKeys = 0, subscriptions = 0, connections = 0, orders = 0;

            foreach (var key in server.Keys(pattern: $"{domain}:*:menu"))
            {
                await db.KeyDeleteAsync(key);
                menuKeys++;
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

            //foreach (var key in server.Keys(pattern: "subscription:*"))
            //{
            //    await db.KeyDeleteAsync(key);
            //    subscriptions++;
            //}

            //foreach (var key in server.Keys(pattern: "*:connection"))
            //{
            //    await db.KeyDeleteAsync(key);
            //    connections++;
            //}

            //foreach (var key in server.Keys(pattern: "order:*"))
            //{
            //    await db.KeyDeleteAsync(key);
            //    orders++;
            //}
            return Ok(new { Menu = menuKeys, DAndP = dAndPKeys, Pending = pendingKeys, Subscriptions = subscriptions, Connections = connections, Orders = orders });
        }

        [HttpGet("import/{companyId:int}")]
        public async Task<IActionResult> Import(int companyId, [FromQuery] string selection = "")
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = new Uri("http://importservice:8080"),
            };

            var response = await httpClient.GetAsync($"import/{companyId}?selection={selection}");

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return StatusCode((int)response.StatusCode, $"Import service encountered an internal error for companyId: {companyId}.\n{response.RequestMessage}");
            }

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Import service failed for companyId: {companyId}");
            }

            return Ok(await response.Content.ReadAsStringAsync());
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
