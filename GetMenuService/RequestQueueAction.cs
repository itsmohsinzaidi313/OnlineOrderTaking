using Microsoft.Extensions.Logging;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace DataService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.DataRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<DataServicePayload>(transport);
            object payload;
            try
            {
                var menuItems = new List<Category>();
                await foreach (var item in GetMenuItemsAsync(requestPayload.RestaurantId))
                {
                    menuItems.Add(item);
                }
                payload = menuItems;
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

        private async IAsyncEnumerable<Category> GetMenuItemsAsync(int companyId)
        {
            logger.LogInformation("📂 Fetching menu items from database...");

            await foreach (var element in impl.GetMenuAsync(companyId: companyId))
            {
                yield return element;
            }
        }
    }
}
