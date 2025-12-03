using System.Text.Json;

namespace PointofSaleModels.ServicePayloads
{
    public static class ServicePayloadExtensions
    {
        private static readonly JsonSerializerOptions _Options = new() { PropertyNameCaseInsensitive = true };
        public static T? GetPayload<T>(this ServicePayload transport)
        {
            if (transport is T typed)
            {
                return typed;
            }

            try
            {
                var json = JsonSerializer.Serialize(transport);
                return JsonSerializer.Deserialize<T>(json, _Options);
            }
            catch
            {
                return default;
            }
        }
    }
}
