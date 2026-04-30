using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Db = PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Protos;
using static PointofSaleModels.Protos.CreateOrderService;
using PointofSaleModels.Application;
using System.Text.Json;

namespace CreateOrderService
{
    public class CreateOrderServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, Implementation impl) : CreateOrderServiceBase
    {
        private readonly JsonSerializerOptions options = new();
        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var order = DeserializeJson(request.OrderJson);
            var connectionString = await GetConnectionString(order.Domain);
            try
            {
                await impl.SaveOrderAsync(connectionString, order!);
                var orderToken = order?.OrderToken;
                if (orderToken == null)
                {
                    return new PlaceOrderResponse { Success = false, Message = "Failed to place order" };
                }
                var response = new PlaceOrderResponse { Success = true, OrderNumber = orderToken, Message = "Order placed successfully" };
                return response;
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return new PlaceOrderResponse { Success = false, Message = $"Error placing order: {message}" };
            }
        }

        public override async Task<PlaceOrderLegacyResponse> PlaceOrderLegacy(PlaceOrderRequest request, ServerCallContext context)
        {
            var placeOrderResponse = await PlaceOrder(request, context);
            var connectionString = await GetConnectionString(request.DomainName);
            var legacyResponse = await impl.GetLegacyResponse(placeOrderResponse.OrderNumber, connectionString);
            return new PlaceOrderLegacyResponse
            {
                Message = placeOrderResponse.Message,
                Success = placeOrderResponse.Success,
                ResponseJson = JsonSerializer.Serialize(legacyResponse)
            };
        }

        private CustomerOrder DeserializeJson(string orderJson)
        {
            options.PropertyNameCaseInsensitive = true;
            var order = JsonSerializer.Deserialize<CustomerOrder>(orderJson, options);
            return order!;
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            var connectionString = restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
            return connectionString.Replace("5434", "5433");
        }
    }
}
