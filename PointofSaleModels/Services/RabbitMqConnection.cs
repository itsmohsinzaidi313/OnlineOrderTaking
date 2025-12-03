using PointofSaleModels.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

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
        /// Ensures an exchange exists on the provided channel.
        /// </summary>
        public static Task EnsureExchangeExistsAsync(IChannel channel, string exchangeName, string type = ExchangeType.Direct, bool durable = true, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
        {
            ArgumentNullException.ThrowIfNull(channel);
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
