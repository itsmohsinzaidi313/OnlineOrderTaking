using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.ServicePayloads;
using System.Security.Claims;

namespace GatewayService
{
    public static class ServicePayloadExtensions
    {
        public static T FillContext<T>(this T payload, HubCallerContext context) where T : ServicePayload
        {
            payload.CorrelationId = Guid.NewGuid().ToString();
            payload.ConnectionId = context.ConnectionId;
            payload.UserId = ExtractUserClaims(context);
            return payload;
        }

        private static string ExtractUserClaims(HubCallerContext context)
        {
            return context.User?.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        }
    }
}
