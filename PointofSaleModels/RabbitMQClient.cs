using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PointofSaleModels
{
    public class RabbitMQClient
    {
        private ConnectionFactory factory;
        private IChannel channel;

        private RabbitMQClient(ConnectionFactory factory, IChannel channel)
        {
            this.factory = factory;
            this.channel = channel;
        }

        public static async Task<RabbitMQClient> Initialize(RabbitMQSettings settings)
        {
            var factory = new ConnectionFactory { HostName = settings.HostName, Port = settings.Port };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            return new RabbitMQClient(factory, channel);
        }

        public async Task DeclareExchangeAsync(string exchange)
        {
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic);
        }

        public async Task DeclareQueueAsync(string queue)
        {
            await channel.QueueDeclareAsync(queue);
        }

        public async Task BindQueueAsync(string exchange, string queue, string routingKey)
        {
            await channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: routingKey);
        }

        public async Task PublishNotificationAsync(string routingKey, NotificationDeliveryTransport transport)
        {
            var body = JsonSerializer.Serialize(transport);
            var bytes = Encoding.UTF8.GetBytes(body);
            await channel.BasicPublishAsync(exchange: ExchangeType.Topic, routingKey: routingKey, bytes);
        }

        public async Task SubscribeAsync(string queue, Action<NotificationDeliveryTransport> action)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var transport = JsonSerializer.Deserialize<NotificationDeliveryTransport>(message);
                if (transport is not null)
                    action(transport);
                return Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue, autoAck: true, consumer: consumer);
        }
    }
}
