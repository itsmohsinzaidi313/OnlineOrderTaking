using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace SettingsDataService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.SettingRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                payload = await GetDeliveryAndPickupItemsAsync(connectionString);
                success = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch data.");
                success = false;
                payload = new
                {
                    Success = false,
                    Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
            }
            var response = new DataServicePayload(requestPayload)
            {
                Success = success,
                DataPayload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.SettingResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = contextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString.Replace("haproxy", "localhost") ?? throw new Exception("Restaurant not found");
        }

        private async Task<JsonObject> GetDeliveryAndPickupItemsAsync(string connectionString)
        {
            logger.LogInformation("🚚 Fetching delivery and pickup items from database...");
            return await impl.GetDataOneAsync(connectionString: connectionString);
        }
    }
}
