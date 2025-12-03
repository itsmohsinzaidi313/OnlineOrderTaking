using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PointofSaleModels.Services
{
    public class RabbitMqPublisher(RabbitMqConnection conn) : IRabbitMqPublisher
    {
        public async Task PublishToQueueAsync<T>(string queueName, T message, string? correlationId = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default)
        {
            await PublishAsync(message, queueName, string.Empty, correlationId, null, headers, cancellationToken);
        }


        /// <summary>
        /// Generalized publish method supporting exchange, routing, correlation and headers.
        /// </summary>
        private async Task PublishAsync<T>(T message, string routingKey, string exchange = "", string? correlationId = null, string? replyTo = null, IDictionary<string, object?>? headers = null, CancellationToken cancellationToken = default)
        {
            // Use a scoped channel for publishing to avoid thread-safety issues.
            var channel = await conn.CreateChannelAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(exchange))
                {
                    // Ensure the destination queue exists when using the default exchange.
                    await conn.EnsureQueueExistsAsync(routingKey);
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
                try { await channel?.CloseAsync(cancellationToken); } catch { /* ignore */ }
            }
        }
    }
}
