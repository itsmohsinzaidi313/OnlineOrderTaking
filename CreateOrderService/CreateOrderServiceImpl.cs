using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Db = PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Protos;
using static PointofSaleModels.Protos.CreateOrderService;
using PointofSaleModels.Application;

namespace CreateOrderService
{
    public class CreateOrderServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, Implementation impl) : CreateOrderServiceBase
    {
        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var order = System.Text.Json.JsonSerializer.Deserialize<CustomerOrder>(request.OrderJson);
            var connectionString = await GetConnectionString(request.DomainName);
            connectionString = connectionString.Replace("5434", "5433");
            await impl.SaveOrderAsync(connectionString, order!);
            var orderToken = order?.OrderToken;
            if(orderToken == null)
            {
                return new PlaceOrderResponse { Success = false, Message = "Failed to place order" };
            }
            var response = new PlaceOrderResponse { Success = true, OrderNumber = orderToken , Message = "Order placed successfully" };
            return response;
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }
    }
}
