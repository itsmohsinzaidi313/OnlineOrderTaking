using Lib.Net.Http.WebPush;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Protos;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace PushNotificationService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, WebPushService pushService, IConnectionMultiplexer multiplexer) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();
        public override string QueueName() => RabbitMqQueues.PushNotificationRequestQueue;

        public override async Task OnMessage(string payload)
        {
            var request = JsonSerializer.Deserialize<PushNotificationServicePayload>(payload);
            var endpoint = multiplexer.GetEndPoints().First();
            var server = multiplexer.GetServer(endpoint);
            var keys = server.Keys(pattern: $"subscription:{request.ClientId}");
            logger.LogInformation("Processing push notification request for pattern {ClientId} Total {Count}", request?.ClientId, keys.Count());

            foreach (var clientId in keys)
            {
                var cid = clientId.ToString();
                var redisValue = await _db.StringGetAsync(cid);
                if (!redisValue.HasValue)
                {
                    logger.LogWarning("No subscription found for client {ClientId}", cid);
                    return;
                }
                var subscription = JsonSerializer.Deserialize<PushSubscriptionDto>(redisValue.ToString());
                if (subscription == null)
                {
                    logger.LogError("Subscription data is corrupted for client {ClientId}", cid);
                    return;
                }
                var pushSubscription = new PushSubscription
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
                try
                {
                    await pushService.SendAsync(pushSubscription, pushMessage);
                    logger.LogInformation("Successfully processed push notification request for client {ClientId}", cid);
                }
                catch (PushServiceClientException clientException)
                {
                    logger.LogError(clientException, "Error sending push notification to client {ClientId}", cid);
                    if (clientException.StatusCode == System.Net.HttpStatusCode.Gone || clientException.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        logger.LogInformation("Removing subscription for client {ClientId} due to invalid endpoint", cid);
                        await _db.KeyDeleteAsync(cid);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing push notification request for client {ClientId}", request.ClientId);
                }
            }
        }
    }
}
