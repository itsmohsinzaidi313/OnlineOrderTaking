using PointofSaleModels.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PointofSaleModels.Services
{
    public class RabbitMqConnection(IOptions<RabbitMqSettings> options) : IAsyncDisposable
    {
        internal readonly RabbitMqSettings _settings = options.Value;
        private IConnection? _connection;
    public IChannel? Channel;


        public async Task InitializeAsync()
        {
            if (Channel != null && _connection != null)
            {
                return;
            }

            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true,
            };

            _connection = await factory.CreateConnectionAsync();
            Channel = await _connection.CreateChannelAsync();
        }

        /// <summary>
        /// Creates a new channel from the underlying connection. Callers are responsible for closing the channel.
        /// </summary>
        public async Task<IChannel> CreateChannelAsync()
        {
            if (_connection == null)
            {
                await InitializeAsync();
            }

            if (_connection == null)
            {
                throw new InvalidOperationException("Unable to create channel: connection is not initialized.");
            }

            return await _connection.CreateChannelAsync();
        }

        public async Task EnsureQueueExistsAsync(string queueName)
        {
            if (Channel == null)
            {
                throw new InvalidOperationException("Channel is not initialized. Call InitializeAsync() first.");
            }

            await Channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        /// <summary>
        /// Ensures the queue exists on the provided channel with the specified durability and arguments.
        /// </summary>
    public static Task EnsureQueueExistsAsync(IChannel channel, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.QueueDeclareAsync(
                queue: queueName,
                durable: durable,
                exclusive: exclusive,
                autoDelete: autoDelete,
                arguments: arguments
            );
        }

        /// <summary>
        /// Ensures an exchange exists on the provided channel.
        /// </summary>
    public static Task EnsureExchangeExistsAsync(IChannel channel, string exchangeName, string type = ExchangeType.Direct, bool durable = true, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: type,
                durable: durable,
                autoDelete: autoDelete,
                arguments: arguments
            );
        }

        /// <summary>
        /// Binds a queue to an exchange with a routing key.
        /// </summary>
        public static Task BindQueueAsync(IChannel channel, string queueName, string exchangeName, string routingKey)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey
            );
        }

        /// <summary>
        /// Generalized publish method supporting exchange, routing, correlation and headers.
        /// </summary>
        public async Task PublishAsync<T>(T message, string routingKey, string exchange = "", string? correlationId = null, string? replyTo = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default)
        {
            // Use a scoped channel for publishing to avoid thread-safety issues.
            var channel = await CreateChannelAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(exchange))
                {
                    // Ensure the destination queue exists when using the default exchange.
                    await EnsureQueueExistsAsync(channel, routingKey);
                }

                var props = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                    CorrelationId = correlationId,
                    ReplyTo = replyTo,
                    Headers = headers
                };

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                    exchange: exchange ?? string.Empty,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: props,
                    body: body,
                    cancellationToken: cancellationToken
                );
            }
            finally
            {
                try { await channel.CloseAsync(); } catch { /* ignore */ }
            }
        }

        /// <summary>
        /// Backwards-compatible convenience wrapper for publishing to a queue via the default exchange.
        /// </summary>
        [Obsolete("Use PublishAsync with exchange parameter instead.")]
        public Task PublishResponseAsync<T>(T response, string routingKey)
        {
            return PublishAsync(response, routingKey);
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (Channel != null)
                await Channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
            Channel = null;
            _connection = null;
        }
    }
}
