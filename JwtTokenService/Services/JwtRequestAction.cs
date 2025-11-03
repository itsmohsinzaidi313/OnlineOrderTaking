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
        var userId = $"{Guid.NewGuid().ToString("N")[..4]}-{Guid.NewGuid().ToString("N")[..4]}";
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
}
