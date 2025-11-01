using PointofSaleModels.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PointofSaleModels.Services
{
    public class RabbitMqConnection(IOptions<RabbitMqSettings> options) : IAsyncDisposable
    {
        internal readonly RabbitMqSettings _settings = options.Value;
        private IConnection? _connection;
        private IChannel? _channel;

        public IChannel Channel => _channel ?? throw new InvalidOperationException("RabbitMQ channel not initialized");

        public async Task InitializeAsync()
        {
            if (_channel != null && _connection != null)
            {
                return;
            }

            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
            };

            // ✅ Use new async connection method
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            var requestQueue = !string.IsNullOrWhiteSpace(_settings.RequestQueueName)
                ? _settings.RequestQueueName!
                : "gateway-requests-queue";

            var responseQueue = !string.IsNullOrWhiteSpace(_settings.ResponseQueueName)
                ? _settings.ResponseQueueName!
                : "gateway-responses-queue";

            // Declare both queues to ensure existence
            await _channel.QueueDeclareAsync(
                queue: requestQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await _channel.QueueDeclareAsync(
                queue: responseQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public async Task PublishResponseAsync<T>(T response)
        {
            await InitializeAsync();

            var props = new BasicProperties();

            var routingKey = _settings.ResponseQueueName!;

            var json = JsonSerializer.Serialize(response);
            var body = Encoding.UTF8.GetBytes(json);
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
