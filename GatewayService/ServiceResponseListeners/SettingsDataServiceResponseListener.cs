using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class SettingsDataServiceResponseListener(ILogger<SettingsDataServiceResponseListener> logger, RabbitMqConnection rabbitConnection, SettingsDataServiceResponseAction listener) : RabbitMqConsumerService<SettingsDataServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
