using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PointofSaleModels.Application;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

namespace GetMenuService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, IDatabase redis) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuRequestQueue;

        public async Task OnMessage(RabbitMqTransport transport)
        {
            object payload;
            try
            {
                var responsePayload = await GetMenuItemsAsync(transport.CompanyId);
                payload = responsePayload;
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
            redis.StringSet(transport.UserId, payload);
            var response = new RabbitMqTransport
            {
                ConnectionId = transport.ConnectionId,
                UserId = transport.UserId,
                Route = "menu.response",
                CompanyId = transport.CompanyId,
                BranchId = transport.BranchId,
                Payload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.MenuResponseQueue, response);
        }

        private async IAsyncEnumerable<Category> GetMenuItemsAsync(string companyId)
        {
            logger.LogInformation("📂 Fetching menu items from database...");

            var results = new List<object>();

            await foreach (var element in impl.GetMenuAsync(companyId: 1))
            {
                yield return element;
            }
        }
    }
}
