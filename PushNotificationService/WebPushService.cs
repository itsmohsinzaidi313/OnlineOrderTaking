using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;
using PointofSaleModels.Settings;

namespace PushNotificationService
{
    public class WebPushService
    {
        private readonly PushServiceClient _client;

        public WebPushService(VapidSettings config)
        {
            _client = new PushServiceClient
            {
                DefaultAuthentication = new VapidAuthentication(
                    config.PublicKey,
                    config.PrivateKey)
                {
                    Subject = config.Subject
                }
            };

        }

        public WebPushService(IOptions<VapidSettings> config)
        {
            _client = new PushServiceClient
            {
                DefaultAuthentication = new VapidAuthentication(
                    config.Value.PublicKey,
                    config.Value.PrivateKey)
                {
                    Subject = config.Value.Subject
                }
            };
        }

        public async Task SendAsync(PushSubscription sub, PushMessage message, CancellationToken cancellationToken = default)
        {
            await _client.RequestPushMessageDeliveryAsync(sub, message, cancellationToken);
        }
    }
}
