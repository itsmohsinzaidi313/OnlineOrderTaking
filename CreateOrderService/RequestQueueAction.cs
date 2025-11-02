using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.DatabaseModels;
using ValueType = PointofSaleModels.Application.ValueType;

namespace CreateOrderService
{
    internal class RequestQueueAction(Db.RestaurantErpWebContext dbContext) : IQueueAction
    {

        public string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public async Task OnMessage(RabbitMqTransport transport)
        {
            CustomerOrder order = transport.Payload as CustomerOrder;
            await SaveOrderAsync(transport.CompanyId, transport.BranchId, order);
        }

        internal async Task<string> SaveOrderAsync(int companyId, int branchId, CustomerOrder order)
        {
            var orderMaster = await GetOrderMasterAsync(companyId, branchId, order);
            return await SaveOrderAsync(orderMaster);
        }

        private async Task<string> SaveOrderAsync(Db.OrderMaster orderMaster)
        {
            await dbContext.OrderMasters.AddAsync(orderMaster);
            await dbContext.SaveChangesAsync();
            return orderMaster.OrderNumber;
        }

        private static IDictionary<OrderStatus, int>? _orderStatusIdCache;
        private static IDictionary<int, OrderStatus>? _orderStatusCache;

        private static OrderStatus GetOrderStatus(string orderStatus)
        {
            return orderStatus.Trim().ToLower() switch
            {
                "cancelled" => OrderStatus.Cancelled,
                "intransit" => OrderStatus.InTransit,
                "ready" => OrderStatus.Ready,
                "pending" => OrderStatus.Pending,
                "confirmed" => OrderStatus.Confirmed,
                "delivered" => OrderStatus.Delivered,
                "dispatch" => OrderStatus.Dispatch,
                "preparing" => OrderStatus.Preparing,
                "served" => OrderStatus.Served,
                "paid" => OrderStatus.Paid,
                "onhold" => OrderStatus.OnHold,
                _ => OrderStatus.Undefined
            };
        }
        private async Task InitializeOrderStatusCache()
        {
            if (_orderStatusCache != null) return;

            var dic = await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatus1, x => x.OrderStatusId);
            _orderStatusIdCache = dic.ToDictionary(x => GetOrderStatus(x.Key ?? string.Empty), x => x.Value);
            _orderStatusCache = dic.ToDictionary(x => x.Value, x => GetOrderStatus(x.Key));
        }

        public string GenerateOrderNumber(int branchId)
        {
            var orderNumber = dbContext.OrderMasters
                .Where(x => x.BranchId == branchId)
                .Select(x => x.OrderNumber)
                .Max();
            var now = DateTime.Now;
            if (!string.IsNullOrEmpty(orderNumber))
            {
                var split = (int.Parse(orderNumber.Split('/').Last()) + 1).ToString();
                while (split.Length < 4)
                {
                    split = "0" + split;
                }
                orderNumber = $"{now.Year}{now.Month}{now.Day}/ORD/{split}";
            }
            else
            {
                orderNumber = $"{now.Year}{now.Month}{now.Day}/ORD/0001";
            }
            return orderNumber;
        }

        public bool OrderNumberExists(int branchId, string orderNumber)
        {
            return dbContext.OrderMasters
                .Any(x => x.BranchId == branchId && x.OrderNumber == orderNumber);
        }

        private int GetOrderModeId(string orderMode, int companyId)
        {
            return dbContext.SetupMasterDetails
                            .Where(x => x.SetupDetailName == orderMode && x.CompanyId == companyId)
                            .FirstOrDefault()!.SetupDetailId;
        }

        private async Task<Db.OrderMaster> GetOrderMasterAsync(int companyId, int branchId, CustomerOrder order)
        {
            await InitializeOrderStatusCache();
            var orderSourceId = dbContext.SetupMasterDetails.FirstOrDefault(x => x.SetupDetailName == "POS")?.SetupDetailId ?? null;
            var discount = order.Discount;
            var tax = order.Tax;
            var orderNumber = order.OrderNumber ?? GenerateOrderNumber(branchId);
            if (OrderNumberExists(branchId, orderNumber))
            {
                orderNumber = GenerateOrderNumber(branchId);
            }
            int orderStatusId = _orderStatusIdCache![order.Status ?? OrderStatus.Pending];

            var orderModeId = GetOrderModeId(order.OrderType.ToString(), companyId);
            var subTotal = order.Items.Select(x => x.Variations.Select(x => x.Price).Sum()).Sum();

            var amountWithTax = subTotal + (subTotal * order.Tax?.Value ?? 0.00);

            var orderMaster = new Db.OrderMaster
            {
                CreatedBy = order.User.Id,
                CreatedDate = DateTime.Now,
                OrderSourceId = orderSourceId,
                OrderNumber = orderNumber,
                CompanyId = companyId,
                BranchId = branchId,
                OrderStatusId = orderStatusId,
                OrderModeId = orderModeId,
                OrderDate = DateTime.Now,
                OrderTime = TimeOnly.FromDateTime(DateTime.Now),
                TotalAmountWithoutGst = subTotal,
                TotalAmountWithGst = amountWithTax,
                DiscountAmount = discount?.Type == ValueType.Amount ? discount.Value : 0.00,
                DiscountId = discount?.Id,
                DiscountPercent = discount?.Type == ValueType.Percentage ? discount.Value : 0.00,
                Gstamount = subTotal * (tax?.Value / 100) ?? 0.00,
                Gstid = tax?.Id,
                Gstpercent = tax?.Value ?? 0.00,
                IsActive = true,
                Cover = order.Persons,
                SpecialInstruction = order.Description,
            };
            foreach (var orderDetail in GetOrderDetails(order.User.Id, order.Items))
            {
                orderMaster.OrderDetails.Add(orderDetail);
            }
            return orderMaster;
        }
        private static List<Db.OrderDetail> GetOrderDetails(int userId, List<MenuItem> items)
        {
            var list = new List<Db.OrderDetail>();
            foreach (var item in items)
            {
                var orderDetail = new Db.OrderDetail
                {
                    IsKot = true,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    SpecialInstruction = item.Comment,
                    Quantity = item.Quantity,
                    ItemFoc = item.ItemFOC,
                    RandomId = new Random().Next(8999) + 1000,
                };

                var variation = item.Variations.FirstOrDefault();
                if (variation != null && item.Variations.Count >= 1)
                {
                    orderDetail.ProductDetailId = variation.Id;
                    foreach (var choice in variation.ItemChoices)
                    {
                        foreach (var option in choice.ItemOptions)
                        {
                            list.Add(new Db.OrderDetail
                            {
                                RandomId = orderDetail.RandomId,
                                OrderParentId = variation.Id,
                                DealItemId = choice.Id,
                                ProductDetailId = option.Id,
                                Quantity = choice.Quantity,
                                IsKot = true,
                                IsActive = true,
                                ItemFoc = false,
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                            });
                        }
                    }
                }
                list.Add(orderDetail);
            }
            return list;
        }
    }
}
