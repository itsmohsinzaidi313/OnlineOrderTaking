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
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.DataRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload = null;
            var success = false;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                if (requestPayload.DataRequestType == "DeliveryAndPickup")
                {
                    payload = await GetDeliveryAndPickupItemsAsync(connectionString);
                    success = true;
                }
                else if (requestPayload.DataRequestType == "Menu")
                {
                    var menuItems = new List<Category>();
                    await foreach (var item in GetMenuItemsAsync(connectionString, requestPayload.BranchId))
                    {
                        menuItems.Add(item);
                    }
                    payload = menuItems;
                    success = true;
                }
                else if (requestPayload.DataRequestType == "Orders")
                {
                    if (!requestPayload.OrderUserId.HasValue) throw new Exception("UserId missing for userwise orders list");

                    var orders = new List<CustomerOrder>();
                    await foreach (var order in impl.GetOrdersAsync(connectionString, requestPayload.OrderUserId.Value))
                    {
                        orders.Add(order);
                    }

                    var orderStatuses = await impl.GetOrderStatusesAsync(connectionString);
                    payload = new { Orders = orders, OrderStatuses = orderStatuses };
                    success = true;
                }
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
            await publisher.PublishToQueueAsync(RabbitMqQueues.DataResponseQueue, response);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = contextFactory.CreateDbContext();
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
