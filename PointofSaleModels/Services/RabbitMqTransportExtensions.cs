using System.Text.Json;

namespace PointofSaleModels.Services
{
    public static class RabbitMqTransportExtensions
    {
        public static T? GetPayload<T>(this RabbitMqTransport transport)
        {
            if (transport.Payload is JsonElement je)
            {
                try
                {
                    return je.Deserialize<T>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    return default;
                }
            }

            if (transport.Payload is T typed)
            {
                return typed;
            }

            try
            {
                var json = JsonSerializer.Serialize(transport.Payload);
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return default;
            }
        }

        public static RabbitMqTransport WithPayload<T>(this RabbitMqTransport transport, T payload)
        {
            transport.Payload = payload!;
            return transport;
        }
    }
}
