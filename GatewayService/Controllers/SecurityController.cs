using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GatewayService.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("")]
    public class SecurityController(IOptions<JwtSettings> jwtOptions, ILogger<SecurityController> logger) : ControllerBase
    {
        private readonly JwtSettings _jwt = jwtOptions.Value;

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("Gateway Service is healthy.");
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

            return Ok($"Service is running for companyId: {companyId}");
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
