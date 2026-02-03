using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;

namespace DataService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, Db.RestaurantsContext context) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.DataRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                if (requestPayload.DataRequestType == "DeliveryAndPickup")
                {
                    payload = await GetDeliveryAndPickupItemsAsync(connectionString);
                }
                else if (requestPayload.DataRequestType == "Menu")
                {
                    var menuItems = new List<Category>();
                    await foreach (var item in GetMenuItemsAsync(connectionString, requestPayload.BranchId))
                    {
                        menuItems.Add(item);
                    }
                    payload = menuItems;
                }
                else if(requestPayload.DataRequestType == "Orders")
                {
                    var orders = new List<CustomerOrder>();
                    await foreach (var order in impl.GetOrdersAsync(connectionString, requestPayload.BranchId))
                    {
                        orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log full exception (stack trace and inner exceptions) to help diagnose stream/connection issues
                logger.LogError(ex, "Failed to fetch menu items.");
                payload = new
                {
                    error = true,
                    message = "Failed to fetch menu items.",
                    details = ex.ToString()
                };
            }
            var response = new DataServicePayload(requestPayload)
            {
                DataPayload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.DataResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }

        private async Task<JsonObject> GetDeliveryAndPickupItemsAsync(string connectionString)
        {
            logger.LogInformation("🚚 Fetching delivery and pickup items from database...");
            return await impl.GetDataOneAsync(connectionString: connectionString);
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
