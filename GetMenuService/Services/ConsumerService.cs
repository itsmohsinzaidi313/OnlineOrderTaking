using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using GetMenuService.Settings;

namespace GetMenuService.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly RabbitMqConnection _rabbitConnection;
        private readonly RabbitMqSettings _settings;

        public ConsumerService(RabbitMqConnection rabbitConnection, IOptions<RabbitMqSettings> options)
        {
            _rabbitConnection = rabbitConnection;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitConnection.InitializeAsync();

            var channel = _rabbitConnection.Channel;

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
           {
               var message = Encoding.UTF8.GetString(ea.Body.ToArray());
               Console.WriteLine($"📥 Received request: {message}");

               string? connectionId = null;
               string route = "unknown";
               JsonElement payload = default;
               bool hasPayload = false;

               try
               {
                   using var doc = JsonDocument.Parse(message);
                   var root = doc.RootElement;
                   if (root.TryGetProperty("connectionId", out var connEl) && connEl.ValueKind == JsonValueKind.String)
                   {
                       connectionId = connEl.GetString();
                   }
                   if (root.TryGetProperty("route", out var routeEl) && routeEl.ValueKind == JsonValueKind.String)
                   {
                       route = routeEl.GetString() ?? route;
                   }
                   if (root.TryGetProperty("payload", out var pl))
                   {
                       // Clone the element so it no longer depends on the lifetime of 'doc'
                       payload = pl.Clone();
                       hasPayload = true;
                   }
               }
               catch (JsonException)
               {
                   // ignore parse errors; we'll just echo raw
               }

               // Build a trivial response. For a real implementation, fetch the menu here.
               // Ensure the payload is independent of any disposed JsonDocument.
               JsonElement responsePayload = hasPayload
                   ? payload
                   : JsonDocument.Parse("{\"status\":\"ok\"}").RootElement.Clone();
               var responseEnvelope = new
               {
                   connectionId = connectionId,
                   route = route,
                   payload = responsePayload,
                   processedAt = DateTimeOffset.UtcNow
               };
               var json = JsonSerializer.Serialize(responseEnvelope);
               var body = Encoding.UTF8.GetBytes(json);

               await _rabbitConnection.PublishResponseAsync(body);

               await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
           };

            // ✅ Use new API signature with all arguments
            var requestQueue = !string.IsNullOrWhiteSpace(_settings.RequestQueueName)
                ? _settings.RequestQueueName!
                : "gateway-requests-queue";
            await channel.BasicConsumeAsync(
                queue: requestQueue,
                autoAck: false,
                consumerTag: string.Empty,   // let RabbitMQ auto-generate
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            Console.WriteLine($"✅ GetMenuService consumer started. Listening on '{requestQueue}' and replying to 'gateway-responses-queue'.");
        }
    }
}
