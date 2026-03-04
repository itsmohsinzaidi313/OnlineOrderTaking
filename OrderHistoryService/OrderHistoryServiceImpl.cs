using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Protos;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService
{
    public class OrderHistoryServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, Implementation implementation) : PointofSaleModels.Protos.OrderHistoryService.OrderHistoryServiceBase
    {
        public override async Task<OrderHistoryResponse> GetOrderHistory(OrderHistoryRequest request, ServerCallContext context)
        {
            using var restaurantDbContext = contextFactory.CreateDbContext();
            var connectionString = await restaurantDbContext.Restaurants
                                    .Join(restaurantDbContext.OrderTokens, r => r.Id, t => t.RestaurantId, (r, t) => new { r.ConnectionString, t.OrderToken })
                                    .Where(rt => rt.OrderToken == request.OrderToken)
                                    .Select(rt => rt.ConnectionString)
                                    .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(connectionString))
                return new OrderHistoryResponse
                {
                    Success = false,
                    Message = "Order token not found",
                };

            using var dbContext = GetDbContext(connectionString);
            var orderMaster = await dbContext.OrderMasters.FirstOrDefaultAsync(x => x.OrderToken == request.OrderToken);
            if (orderMaster == null)
                return new OrderHistoryResponse
                {
                    Success = false,
                    Message = "Order number not found",
                };
            var userId = orderMaster.CreatedBy;
            var response = new OrderHistoryResponse
            {
                Success = true,
                Message = "Order Found",
            };
            await foreach (var order in implementation.GetOrdersAsync(connectionString, orderToken: request.OrderToken))
            {
                response.OrdersPayload.Add(System.Text.Json.JsonSerializer.Serialize(order));
            }

            return response;
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
