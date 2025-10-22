using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using GetMenuService.Settings;
using PointofSaleModels.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GetMenuService.Services
{
    public class ConsumerService(RabbitMqConnection rabbitConnection, IOptions<RabbitMqSettings> options, RestaurantErpWebContext db) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await rabbitConnection.InitializeAsync();

            var channel = rabbitConnection.Channel;

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
           {
               var message = Encoding.UTF8.GetString(ea.Body.ToArray());
               Console.WriteLine($"📥 Received request: {message}");

               string? connectionId = null;
               string route = "unknown";
               JsonElement payload = default;

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
                   }
               }
               catch (JsonException)
               {
                   // ignore parse errors; we'll just echo raw
               }

               // Build a trivial response. For a real implementation, fetch the menu here.
               // Ensure the payload is independent of any disposed JsonDocument.

               List<object> responsePayload = [];

               try
               {
                   foreach (var item in GetMenuItems())
                   {
                       responsePayload.Add(item);
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Error fetching menu items: {ex.Message}");
               }
               var responseEnvelope = new
               {
                   connectionId,
                   route,
                   payload = responsePayload,
                   processedAt = DateTimeOffset.UtcNow
               };
               var json = JsonSerializer.Serialize(responseEnvelope);
               var body = Encoding.UTF8.GetBytes(json);

               await rabbitConnection.PublishResponseAsync(body);

               await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
           };
            // ✅ Use new API signature with all arguments
            var settings = options.Value;
            var requestQueue = !string.IsNullOrWhiteSpace(settings.RequestQueueName)
                ? settings.RequestQueueName!
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

        private IEnumerable<object> GetMenuItems()
        {
            Console.WriteLine("📂 Fetching menu items from database...");
            var connection = db.Database.GetDbConnection();
            var command = connection.CreateCommand();

            command.CommandText = @"SELECT ""Id"", ""Name"", ""Price"" FROM ""Products""";
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return new
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                };
            }
            Console.WriteLine("✅ Menu items fetched successfully.");
        }
    }
}
