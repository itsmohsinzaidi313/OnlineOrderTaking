// Services/ConsumerService.cs
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Hubs;
using GatewayService.Settings;

namespace GatewayService.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly RabbitMqConnection _rabbitConnection;
    private readonly IHubContext<GatewayHub> _gatewayHub;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(
            RabbitMqConnection rabbitConnection,
            IHubContext<GatewayHub> gatewayHub,
            IOptions<RabbitMqSettings> mqOptions,
            ILogger<ConsumerService> logger)
        {
            _rabbitConnection = rabbitConnection;
            _gatewayHub = gatewayHub;
            _settings = mqOptions.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitConnection.InitializeAsync();

            var channel = _rabbitConnection.Channel;

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
           {
               var message = Encoding.UTF8.GetString(ea.Body.ToArray());
               _logger.LogInformation("📥 Received message: {Message}", message);

               string? connectionId = null;
               JsonElement payloadEl = default;
               bool hasPayload = false;

               try
               {
                   using var doc = JsonDocument.Parse(message);
                   var root = doc.RootElement;
                   if (root.TryGetProperty("connectionId", out var connEl) && connEl.ValueKind == JsonValueKind.String)
                   {
                       connectionId = connEl.GetString();
                   }
                   // prefer `payload` field if present; otherwise send the whole message
                   if (root.TryGetProperty("payload", out var pl))
                   {
                       // Clone to decouple from the owning JsonDocument which is disposed at the end of this block
                       payloadEl = pl.Clone();
                       hasPayload = true;
                   }
               }
               catch (JsonException)
               {
                   // not JSON; we'll forward raw
               }

               if (!string.IsNullOrWhiteSpace(connectionId))
               {
                   var payloadToSend = hasPayload ? (object)payloadEl : message;
                   await _gatewayHub.Clients.Client(connectionId!).SendAsync("Response", payloadToSend, cancellationToken: stoppingToken);
               }
               else
               {
                   _logger.LogWarning("Received message without connectionId; dropping: {Message}", message);
               }

               await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
           };

            // ✅ Use new API signature with all arguments
            var responseQueue = !string.IsNullOrWhiteSpace(_settings.ResponseQueueName)
                ? _settings.ResponseQueueName!
                : "gateway-responses-queue";

            await channel.BasicConsumeAsync(
                queue: responseQueue,
                autoAck: false,
                consumerTag: string.Empty,   // let RabbitMQ auto-generate
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation("✅ RabbitMQ consumer started. Listening for messages on queue {Queue}", responseQueue);
        }
    }
}
