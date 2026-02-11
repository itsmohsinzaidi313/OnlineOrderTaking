using System.Text.Json;

namespace PointofSaleModels.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishToQueueAsync<T>(string queueName, T message, string? correlationId = null, IDictionary<string, object?>? headers = null, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
    }
}
