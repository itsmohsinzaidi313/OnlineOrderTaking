using RabbitMQ.Client;
using GatewayService.Settings;
using Microsoft.Extensions.Options;

namespace GatewayService.Services
{
    public class RabbitMqConnection : IAsyncDisposable
    {
        private readonly RabbitMqSettings _settings;
        private IConnection? _connection;
        private IChannel? _channel;

        public IChannel Channel => _channel ?? throw new InvalidOperationException("RabbitMQ channel not initialized");

        public RabbitMqConnection(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public async Task InitializeAsync()
        {
            if (_channel != null && _connection != null)
            {
                // Already initialized
                return;
            }
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
            };

            // ✅ Use new async connection method
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare queues (separate request/response queues to avoid loopback)
            var requestQueue = !string.IsNullOrWhiteSpace(_settings.RequestQueueName)
                ? _settings.RequestQueueName!
                : "gateway-requests-queue";

            var responseQueue = !string.IsNullOrWhiteSpace(_settings.ResponseQueueName)
                ? _settings.ResponseQueueName!
                : "gateway-responses-queue";

            await _channel.QueueDeclareAsync(
                queue: requestQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Also ensure response queue exists for the consumer service
            await _channel.QueueDeclareAsync(
                queue: responseQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public async Task PublishAsync(ReadOnlyMemory<byte> body)
        {
            await InitializeAsync();

            // Using BasicProperties from RabbitMQ.Client 7.x API
            var props = new BasicProperties();

            var routingKey = !string.IsNullOrWhiteSpace(_settings.RequestQueueName)
                ? _settings.RequestQueueName!
                : "gateway-requests-queue";

            await Channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: default
            );
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
