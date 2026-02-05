using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderStatusService
{
    public class RequestQueueAction(ILogger<RequestQueueAction> logger, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.OrderStatusRequestQueue;

        public async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderStatusPayload>(transport);
            object? payload = null;
            try
            {
                var dbContext = await GetDbContextAsync(requestPayload.DomainName);
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
                    payload = new
                    {
                        Success = true,
                        Message = "Order updated successfully"
                    };
                }
                payload = new
                {
                    Success = false,
                    Message = "No order found"
                };
            }
            catch (Exception ex)
            {
                var message = "Error processing order status request: {Message}" + ex.Message;
                logger.LogError(message);
                payload = new
                {
                    Success = false,
                    message,
                };
            }
            var response = new OrderStatusPayload(requestPayload)
            {
                DataPayload = payload,
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderStatusResponseQueue, response);
        }
        private async Task<Db.PgDbContext> GetDbContextAsync(string domainName)
        {
            var connectionString = await GetConnectionString(domainName);
            connectionString = connectionString.Replace("5434", "5433");
            return GetDbContext(connectionString);
        }
        private async Task<string> GetConnectionString(string domainName)
        {
            var context = await contextFactory.CreateDbContextAsync();
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
