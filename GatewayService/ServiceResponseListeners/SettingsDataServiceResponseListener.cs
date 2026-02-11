using PointofSaleModels.Services;

namespace GatewayService.ServiceResponseListeners
{
    public class DataServiceResponseListener(ILogger<DataServiceResponseListener> logger, RabbitMqConnection rabbitConnection, DataServiceResponseAction listener) : RabbitMqConsumerService<DataServiceResponseListener>(logger, rabbitConnection, listener)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
