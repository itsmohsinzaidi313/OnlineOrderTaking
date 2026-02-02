using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderStatusService
{
    public class RequestQueueAction(ILogger<RequestQueueAction> logger, IRabbitMqPublisher publisher, Db.RestaurantsContext context) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.OrderStatusRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderStatusPayload>(transport);
            object? payload = null;
            try
            {
                var connectionString = await GetConnectionString(requestPayload.DomainName);
                var dbContext = GetDbContext(connectionString);
                var orderMaster = await dbContext.OrderMasters.Where(x => x.OrderMasterId == requestPayload.OrderId).FirstOrDefaultAsync();
                if (orderMaster != null)
                {
                    if (requestPayload.BranchTransferId != null)
                    {
                        orderMaster.BranchId = requestPayload.BranchTransferId.Value;
                        await dbContext.SaveChangesAsync();
                    }

                    if (requestPayload.OrderStatusId != null)
                    {
                        orderMaster.OrderStatusId = requestPayload.OrderStatusId.Value;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing order status request: {Message}", ex.Message);
                logger.LogError(ex, "Failed to fetch menu items.");
                payload = new
                {
                    error = true,
                    message = "Failed to fetch menu items.",
                    details = ex.ToString()
                };
            }
            var response = new OrderStatusPayload(requestPayload)
            {
                DataPayload = payload,
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderStatusResponseQueue, response);
        }
        private async Task<string> GetConnectionString(string domainName)
        {
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
        private static Db.PgDbContext GetDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<Db.PgDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .Options;
            return new Db.PgDbContext(options);
        }
    }
}
