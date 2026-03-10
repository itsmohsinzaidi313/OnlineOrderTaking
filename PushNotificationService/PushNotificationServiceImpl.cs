using Grpc.Core;
using Lib.Net.Http.WebPush;
using PointofSaleModels.Protos;
using PointofSaleModels.ServicePayloads;
using StackExchange.Redis;
using System.Text.Json;
using static PointofSaleModels.Protos.PushNotificationService;

namespace PushNotificationService
{
    public class PushNotificationServiceImpl(ILogger<PushNotificationServiceImpl> logger, WebPushService service, IConnectionMultiplexer multiplexer) : PushNotificationServiceBase
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();
        public override async Task<PushNotificationNotifyResponse> Notify(PushNotificationNotifyRequest request, ServerCallContext context)
        {
            var redisValue = await _db.StringGetAsync($"subscription:{request.ClientId}");
            if (!redisValue.HasValue)
            {
                return new PushNotificationNotifyResponse
                {
                    Success = false,
                    Message = "Subscription not found for the client"
                };
            }
            var subscription = JsonSerializer.Deserialize<PushSubscriptionDto>(redisValue.ToString());
            if (subscription == null)
            {
                return new PushNotificationNotifyResponse
                {
                    Success = false,
                    Message = "Subscription corrupted for the client"
                };
            }
            var pushSubscribtion = new PushSubscription
            {
                Endpoint = subscription.Endpoint,
                Keys = new Dictionary<string, string>
                {
                    { "p256dh", subscription.P256DH },
                    { "auth", subscription.Auth }
                }
            };
            var content = JsonSerializer.Serialize(new
            {
                title = request.Title,
                message = request.Message
            });
            var pushMessage = new PushMessage(content);
            await service.SendAsync(pushSubscribtion, pushMessage);
            logger.LogInformation("Sent notification to client {ClientId} at endpoint {Endpoint}", request.ClientId, subscription.Endpoint);
            var response = new PushNotificationNotifyResponse
            {
                Success = true,
                Message = "Notification sent successfully"
            };
            return response;
        }

        public override async Task<PushNotificationSubscriptionResponse> Subscribe(PushNotificationSubscriptionRequest request, ServerCallContext context)
        {
            var clientId = request.ClientId;
            var pushSubscribtion = new PushSubscriptionDto
            {
                Endpoint = request.Endpoint,
                ClientId = request.ClientId,
                P256DH = request.P256Dh,
                Auth = request.Auth
            };
            var json = System.Text.Json.JsonSerializer.Serialize(pushSubscribtion);
            await _db.StringSetAsync($"subscription:{clientId}", json);
            var redisValue = await _db.StringGetAsync($"subscription:{clientId}");
            logger.LogInformation("Stored subscription for client {ClientId} ({HasValue})", clientId, redisValue.HasValue);
            var response = new PushNotificationSubscriptionResponse
            {
                Success = true,
                Message = "Subscribed successfully"
            };
            return response;
        }

        public override async Task<PushNotificationUnsubscribeResponse> Unsubscribe(PushNotificationUnsubscribeRequest request, ServerCallContext context)
        {
            var clientId = request.ClientId;
            await _db.KeyDeleteAsync($"subscription:{clientId}");
            var response = new PushNotificationUnsubscribeResponse
            {
                Success = true,
                Message = "Unsubscribed successfully"
            };
            return response;
        }
    }
}
