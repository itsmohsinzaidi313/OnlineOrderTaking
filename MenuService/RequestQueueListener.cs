using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Services;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace MenuService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher, IConnectionStringResolver resolver) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.MenuRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await resolver.ResolveAsync(requestPayload.DomainName);
                payload = await GetMenuItemsAsync(connectionString, requestPayload.BranchId).ToListAsync();
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
            await publisher.PublishToQueueAsync(RabbitMqQueues.MenuResponseQueue, response);
        }
        
        private async IAsyncEnumerable<Category> GetMenuItemsAsync(string connectionString, int branchId)
        {
            logger.LogInformation("📂 Fetching menu items from database...");

            await foreach (var element in impl.GetMenuAsync(connectionString: connectionString, branchId: branchId))
            {
                yield return element;
            }
        }
    }
}
