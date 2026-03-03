using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Protos;
using App = PointofSaleModels.Application;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService
{
    public class OrderHistoryServiceImpl(IDbContextFactory<Db.RestaurantsContext> contextFactory, Implementation implementation) : PointofSaleModels.Protos.OrderHistoryService.OrderHistoryServiceBase
    {
        public override async Task<OrderHistoryResponse> GetOrderHistory(OrderHistoryRequest request, ServerCallContext context)
        {
            using var restaurantDbContext = contextFactory.CreateDbContext();
            var connectionString = restaurantDbContext.Restaurants
                                    .Join(restaurantDbContext.OrderTokens, r => r.Id, t => t.RestaurantId, (r, t) => new { r.ConnectionString, t.OrderToken })
                                    .Where(rt => rt.OrderToken == request.OrderToken)
                                    .Select(rt => rt.ConnectionString)
                                    .FirstOrDefault();

            using var dbContext = GetDbContext(connectionString);
            var orderMaster = await dbContext.OrderMasters.FirstOrDefaultAsync(x => x.OrderToken == request.OrderToken);
            var orderNumber = orderMaster?.OrderNumber ?? throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));
            var userId = orderMaster.CreatedBy;
            var response = new OrderHistoryResponse();
            await foreach (var order in implementation.GetOrdersAsync(connectionString, userId, orderNumber))
            {
                var protoOrder = new CustomerOrder
                {
                    Domain = order.Domain ?? string.Empty,
                    BranchId = order.BranchId,
                    BranchName = order.BranchName ?? string.Empty,
                    OrderNumber = order.OrderNumber ?? string.Empty,
                    OrderToken = order.OrderToken ?? string.Empty,
                    OrderType = order.OrderType ?? string.Empty,
                    PaymentType = order.PaymentType ?? string.Empty,
                    Status = order.Status ?? string.Empty,
                    AmountWithGst = order.AmountWithGst,
                    AmountWithoutGst = order.AmountWithoutGst,
                    
                };

                foreach (var item in GetMenuItems(order.Items))
                {
                    protoOrder.Items.Add(item);
                }



                if (order.PaymentStatus.HasValue)
                {
                    protoOrder.PaymentStatus = (PointofSaleModels.Protos.PaymentStatus)order.PaymentStatus.Value;
                }

                response.Orders.Add(protoOrder);
            }

            return response;
        }

        private IEnumerable<MenuItem> GetMenuItems(List<App.MenuItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var protoItem = new MenuItem
                {
                    Name = item.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    CategoryId = item.CategoryId,
                    Code = item.Code ?? string.Empty,
                    Comment = item.Comment ?? string.Empty,
                    DepartmentName = item.DepartmentName ?? string.Empty,
                    Description = item.Description ?? string.Empty,
                    Id = item.Id,
                    Image = item.Image ?? string.Empty,
                    IsKot = item.IsKot,
                    Price = item.Price,
                    TaxAmount = item.TaxAmount,
                };

                var itemDiscount = new Discount
                {
                    Id = item.Discount?.Id ?? 0,
                    Name = item.Discount?.Name ?? string.Empty,
                    Type = item.Discount?.Type ?? string.Empty,
                    MaxCap = item.Discount?.MaxCap ?? 0,
                    Value = item.Discount?.Value ?? 0,
                    MinCap = item.Discount?.MinCap ?? 0,
                };
                protoItem.Discount = itemDiscount;
                item.Variations?.ForEach(variation =>
                {
                    var variationDiscount = new Discount
                    {
                        Id = variation.Discount?.Id ?? 0,
                        Name = variation.Discount?.Name ?? string.Empty,
                        Type = variation.Discount?.Type ?? string.Empty,
                        MaxCap = variation.Discount?.MaxCap ?? 0,
                        Value = variation.Discount?.Value ?? 0,
                        MinCap = variation.Discount?.MinCap ?? 0,
                    };
                    var flavour = new ItemFlavour
                    {
                        Id = variation.Flavour?.Id ?? 0,
                        Name = variation.Flavour?.Name ?? string.Empty,
                    };

                    var size = new ItemSize
                    {
                        Id = variation.Size?.Id ?? 0,
                        Name = variation.Size?.Name ?? string.Empty,
                    };
                    var protoVariation = new ItemVariation
                    {
                        Discount = variationDiscount,
                        Id = variation.Id,
                        Price = variation.Price,
                        Flavour = flavour,
                        Size = size,
                    };

                    variation.ItemChoices?.ForEach(choice =>
                    {
                        var protoChoice = new ItemChoice
                        {
                            Id = choice.Id,
                            Name = choice.Name ?? string.Empty,
                            MaxChoice = choice.MaxChoice,
                            Quantity = choice.Quantity,
                        };

                        foreach (var option in choice.ItemOptions)
                        {
                            var protoOption = new ItemOption
                            {
                                Id = option.Id,
                                Name = option.Name ?? string.Empty,
                                Price = option.Price,
                                Quantity = option.Quantity ?? 0,
                            };
                            protoChoice.ItemOptions.Add(protoOption);
                        }

                        protoVariation.ItemChoices.Add(protoChoice);
                    });


                    protoItem.Variations.Add(protoVariation);
                });

                yield return protoItem;
            }
        }

        private Db.PgDbContext GetDbContext(string connectionString)
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
