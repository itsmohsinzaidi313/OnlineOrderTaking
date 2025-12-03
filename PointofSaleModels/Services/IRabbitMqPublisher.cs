namespace PointofSaleModels.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishToQueueAsync<T>(string queueName, T message, string? correlationId = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default);
    }
}
