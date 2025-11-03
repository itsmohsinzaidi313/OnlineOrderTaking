using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json;

namespace JwtTokenService.Services;

public class JwtRequestAction(TokenService tokenService, RabbitMqConnection connection, ILogger<JwtRequestAction> logger) : IQueueAction
{
    public string QueueName() => RabbitMqQueues.JwtRequestQueue;

    public async Task OnMessage(RabbitMqTransport transport)
    {
        logger.LogInformation("JwtTokenService: Received message for route {Route}", transport.Route);

        string? userId = ExtractUserId(transport.Payload);
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning("JwtTokenService: userId not found in payload for connection {Conn}", transport.ConnectionId);
            return;
        }

        var token = tokenService.GenerateToken(userId);

        var response = new RabbitMqTransport
        {
            ConnectionId = transport.ConnectionId,
            // Include the target user id so the Gateway can route the response to the correct user
            UserId = userId,
            Route = "jwt.response",
            CompanyId = transport.CompanyId,
            BranchId = transport.BranchId,
            Payload = token
        };

        await connection.PublishResponseAsync(response, RabbitMqQueues.JwtResponseQueue);
        logger.LogInformation("JwtTokenService: Published token response for user {User}", userId);
    }

    private static string? ExtractUserId(object? payload)
    {
        if (payload == null) return null;

        // If payload is JsonElement (common when deserializing with System.Text.Json)
        if (payload is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.String)
                return je.GetString();

            if (je.ValueKind == JsonValueKind.Object)
            {
                if (je.TryGetProperty("userId", out var p) || je.TryGetProperty("UserId", out p) || je.TryGetProperty("id", out p))
                {
                    return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
                }
            }
        }

        // If payload is plain string containing JSON
        if (payload is string s)
        {
            try
            {
                using var doc = JsonDocument.Parse(s);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.String)
                    return root.GetString();
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("userId", out var p) || root.TryGetProperty("UserId", out p) || root.TryGetProperty("id", out p))
                    {
                        return p.ValueKind == JsonValueKind.String ? p.GetString() : p.ToString();
                    }
                }
            }
            catch
            {
                // not JSON, treat the whole string as userId
                return s;
            }
        }

        // Fallback: try ToString()
        return payload.ToString();
    }
}
