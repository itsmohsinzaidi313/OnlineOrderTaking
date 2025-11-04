namespace PointofSaleModels.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, string routingKey, string exchange = "", string? correlationId = null, string? replyTo = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default);
        Task PublishToQueueAsync<T>(string queueName, T message, string? correlationId = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default);
    }
}
