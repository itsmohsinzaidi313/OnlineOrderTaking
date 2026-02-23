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
            try
            {
                var redisValue = await _db.StringGetAsync($"subscription:{request.ClientId}");
                if (!redisValue.HasValue)
                {
                    logger.LogWarning("No subscription found for client {ClientId}", request.ClientId);
                    return;
                }
                var subscription = JsonSerializer.Deserialize<PushSubscriptionDto>(redisValue.ToString());
                if (subscription == null)
                {
                    logger.LogError("Subscription data is corrupted for client {ClientId}", request.ClientId);
                    return;
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
                await pushService.SendAsync(pushSubscribtion, pushMessage);
                logger.LogInformation("Successfully processed push notification request for client {ClientId}", request.ClientId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing push notification request for client {ClientId}", request.ClientId);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
