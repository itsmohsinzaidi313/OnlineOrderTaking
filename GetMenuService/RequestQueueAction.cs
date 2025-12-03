using Microsoft.Extensions.Logging;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GetMenuService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuRequestQueue;

        public async Task OnMessage(ServicePayload transport)
        {
            var requestPayload = transport.GetPayload<GetMenuServicePayload>();
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
            var response = new GetMenuServicePayload(requestPayload)
            {
                Menu = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.MenuResponseQueue, response);
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
