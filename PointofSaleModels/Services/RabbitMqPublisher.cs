namespace PointofSaleModels.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly RabbitMqConnection _conn;

        public RabbitMqPublisher(RabbitMqConnection conn)
        {
            _conn = conn;
        }

        public Task PublishAsync<T>(T message, string routingKey, string exchange = "", string? correlationId = null, string? replyTo = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default)
        {
            return _conn.PublishAsync(message, routingKey, exchange, correlationId, replyTo, headers, cancellationToken);
        }

        public Task PublishToQueueAsync<T>(string queueName, T message, string? correlationId = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default)
        {
            return _conn.PublishAsync(message, queueName, string.Empty, correlationId, null, headers, cancellationToken);
        }
    }
}
