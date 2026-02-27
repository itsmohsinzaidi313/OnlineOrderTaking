using Grpc.Core;
using PointofSaleModels.Protos;

namespace OrderHistoryService
{
    public class OrderHistoryServiceImpl(Implementation implementation) : PointofSaleModels.Protos.OrderHistoryService.OrderHistoryServiceBase
    {
        public override Task<OrderHistoryResponse> GetOrderHistory(OrderHistoryRequest request, ServerCallContext context)
        {
            return base.GetOrderHistory(request, context);
        }
    }
}
