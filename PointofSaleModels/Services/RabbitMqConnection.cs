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
            };

            // ✅ Use new async connection method
            _connection = await factory.CreateConnectionAsync();
            Channel = await _connection.CreateChannelAsync();
        }

        public async Task PublishResponseAsync<T>(T response, string routingKey)
        {
            await Channel!.QueueDeclareAsync(
                queue: routingKey,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            var props = new BasicProperties();

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
            if (Channel != null)
                await Channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
