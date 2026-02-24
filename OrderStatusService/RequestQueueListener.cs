using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderStatusService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IRabbitMqPublisher publisher, IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderStatusRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderStatusPayload>(transport);
            object? payload = null;
            try
            {
                var dbContext = await GetDbContextAsync(requestPayload.DomainName);
                var orderMaster = await dbContext.OrderMasters.Where(x => x.OrderNumber == requestPayload.OrderNumber).FirstOrDefaultAsync();
                if (orderMaster != null)
                {
                    if (requestPayload.BranchTransferId != null)
                    {
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.BranchId, requestPayload.BranchTransferId.Value));
                    }

                    if (requestPayload.OrderStatusId != null)
                    {
                        orderMaster.OrderStatusId = requestPayload.OrderStatusId.Value;
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.OrderStatusId, requestPayload.OrderStatusId));

                        var previousOrderStatusLog = await dbContext.OrderStatusLogs
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId && x.OrderStatusId == requestPayload.OrderStatusId)
                            .FirstOrDefaultAsync();
                        if (previousOrderStatusLog == null)
                        {
                            dbContext.OrderStatusLogs.Add(new Db.OrderStatusLog
                            {
                                OrderMasterId = orderMaster.OrderMasterId,
                                OrderStatusId = requestPayload.OrderStatusId.Value,
                                CompanyId = orderMaster.CompanyId,
                                Description = string.Empty,
                                CreatedDateTime = DateTime.UtcNow,
                            });
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    if (requestPayload.RiderId != null)
                    {
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.RiderId, requestPayload.RiderId));
                    }
                    payload = new
                    {
                        Success = true,
                        Message = "Order updated successfully"
                    };
                }
                else
                {
                    payload = new
                    {
                        Success = false,
                        Message = "No order found"
                    };
                }

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
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
