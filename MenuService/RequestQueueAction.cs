using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace MenuService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                var menuItems = new List<Category>();
                await foreach (var item in GetMenuItemsAsync(connectionString, requestPayload.BranchId))
                {
                    menuItems.Add(item);
                }
                payload = menuItems;
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

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = contextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
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

    enum RequestType
    {
        Menu, DeliveryAndPickup
    }
}
