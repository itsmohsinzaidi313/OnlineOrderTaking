using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Protos;
using PointofSaleModels.Services;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService
{
    public class OrderHistoryServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, IRestaurantDbContextFactory restaurantDbContextFactory, Implementation implementation) : PointofSaleModels.Protos.OrderHistoryService.OrderHistoryServiceBase
    {
        public override async Task<OrderHistoryResponse> GetOrderHistory(OrderHistoryRequest request, ServerCallContext context)
        {
            using var restaurantDbContext = contextFactory.CreateDbContext();
            var url = await restaurantDbContext.Restaurants
                                    .Join(restaurantDbContext.OrderTokens, r => r.Id, t => t.RestaurantId, (r, t) => new { r.DomainName, t.OrderToken })
                                    .Where(rt => rt.OrderToken == request.OrderToken)
                                    .Select(rt => rt.DomainName)
                                    .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(url))
                return new OrderHistoryResponse
                {
                    Success = false,
                    Message = "Order token not found",
                };

            using var dbContext = await restaurantDbContextFactory.CreateDbContextByUrlAsync(url);
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
            await foreach (var order in implementation.GetOrdersAsync(url, orderToken: request.OrderToken))
            {
                response.OrdersPayload.Add(System.Text.Json.JsonSerializer.Serialize(order));
            }

            return response;
        }
    }
}
