using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace ClientNotificationService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IRabbitMqPublisher publisher,
        IDbContextFactory<Db.RestaurantsContext> contextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ClientNotificationRequestQueue;
        public override async Task OnMessage(string payload)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<ClientNotificationServicePayload>(payload);
            object? response = null;
            try
            {
                var connectionString = await GetConnectionString(requestPayload!.DomainName);
                using var dbContext = GetDbContext(connectionString);
                var userIds = await dbContext.UserBranchMappings
                    .Where(x => x.BranchId == requestPayload.CustomerOrder.BranchId)
                    .Select(x => x.UserId)
                    .ToListAsync();

                requestPayload.NewOrderNotificationKeys = [.. userIds.Select(x => $"branch:{x}:*:connection")];
                await publisher.PublishToQueueAsync(RabbitMqQueues.ClientNotificationGatewayResponse, requestPayload);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order notification request for domain {DomainName}", requestPayload?.DomainName);
            }
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
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
