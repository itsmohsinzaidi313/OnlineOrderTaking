using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderHistoryService;

public class Implementation()
{
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

    internal async IAsyncEnumerable<CustomerOrder> GetOrdersAsync(string connectionString, int? userId = null, string? orderToken = null)
    {
        using var dbContext = GetDbContext(connectionString);
        var products = await (from x in dbContext.ProductCategories
                              join y in dbContext.Products on x.CategoryId equals y.ProductCategoryId
                              select y).ToListAsync();
        var productDetails = await (from a in dbContext.ProductDetails
                                    join b in dbContext.Products on a.ProductId equals b.ProductId
                                    join c in dbContext.ProductCategories on b.ProductCategoryId equals c.CategoryId
                                    select a).ToListAsync();
        var dealItems = await (from a in dbContext.DealItemDetails
                               join b in dbContext.ProductDetails on a.ProductDetailId equals b.ProductDetailId
                               join c in dbContext.Products on b.ProductId equals c.ProductId
                               join d in dbContext.ProductCategories on c.ProductCategoryId equals d.CategoryId
                               select a).ToListAsync();
        var dealDescriptions = await (from a in dbContext.DealDescriptions
                                      join b in dbContext.DealItemDetails on a.DealItemId equals b.DealItemId
                                      join c in dbContext.ProductDetails on b.ProductDetailId equals c.ProductDetailId
                                      join d in dbContext.Products on c.ProductId equals d.ProductId
                                      join e in dbContext.ProductCategories on d.ProductCategoryId equals e.CategoryId
                                      select a).ToListAsync();
        var flavours = await dbContext.Flavours.ToDictionaryAsync(x => x.FlavourId, x => x);
        var sizes = await dbContext.ProductSizes.ToDictionaryAsync(x => x.SizeId, x => x);
        var setupDetail = await dbContext.SetupMasterDetails.ToDictionaryAsync(x => x.SetupDetailId, x => x.SetupDetailName);
        var statuses = await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
        var branchDict = await dbContext.BranchMasters.ToDictionaryAsync(x => x.BranchId, x => x.BranchName);
        var discounts = await dbContext.Discounts.ToDictionaryAsync(x => x.DiscountId, x => x);
        var riders = await dbContext.Riders.ToListAsync();
        if (userId.HasValue)
        {
            var branchIds = await dbContext.UserBranchMappings.Where(x => x.UserId == userId).Select(x => x.BranchId).ToListAsync();
            foreach (var branchId in branchIds)
            {
                async Task<List<Db.OrderMaster>> allOrderMastersAsync() => await dbContext.OrderMasters.Where(x => x.BranchId == branchId && x.OrderDate > DateOnly.FromDateTime(DateTime.Now.AddDays(-3))).ToListAsync();
                var orderMasters = await allOrderMastersAsync();
                await foreach (var order in ordersAsync(orderMasters))
                {
                    yield return order;
                }

            }
        }
        else if (!string.IsNullOrEmpty(orderToken))
        {
            var orderMasters = await dbContext.OrderMasters.Where(x => x.OrderToken == orderToken).ToListAsync();
            await foreach (var order in ordersAsync(orderMasters))
            {
                yield return order;
            }
        }

        async IAsyncEnumerable<CustomerOrder> ordersAsync(IEnumerable<Db.OrderMaster> orderMasters)
        {
            foreach (var dbOrder in orderMasters)
            {
                var orderTime = dbOrder.OrderTime;
                var orderDate = dbOrder.OrderDate;
                DateTime? orderDateTime = orderDate?.ToDateTime(orderTime);
                var orderStatusLogs = await dbContext.OrderStatusLogs.Where(x => x.OrderMasterId == dbOrder.OrderMasterId).ToListAsync();

                // Find Asia/Karachi timezone once per order
                var karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");

                var order = new CustomerOrder
                {
                    OrderNumber = dbOrder.OrderNumber ?? "N/A",
                    OrderToken = dbOrder.OrderToken ?? "N/A",
                    BranchId = dbOrder.BranchId,
                    BranchName = branchDict[dbOrder.BranchId],
                    OrderType = setupDetail[dbOrder.OrderModeId!.Value],
                    Status = statuses[dbOrder.OrderStatusId],
                    Items = [],
                    DeliveryCharges = (int?)(dbOrder.DeliveryCharges ?? 0.00),
                    AmountWithoutGst = dbOrder.TotalAmountWithoutGst ?? 0.00,
                    AmountWithGst = dbOrder.TotalAmountWithGst ?? 0.00,
                    OrderTime = orderDateTime ?? DateTime.MinValue,
                    OrderStatusLogs = orderStatusLogs.Select(x => new
                    {
                        Id = dbOrder.OrderStatusId,
                        CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.SpecifyKind(x.CreatedDateTime, DateTimeKind.Utc),
                            karachiTz
                        ),
                    }).ToList(),
                    Rider = riders.Select(x => new Rider { Id = x.RiderId, Name = x.RiderName, Contact = x.Contact1 }).FirstOrDefault(x => x.Id == dbOrder.RiderId)
                };
                if (dbOrder.DiscountId.HasValue && dbOrder.DiscountId != 0)
                {
                    var disc = discounts[dbOrder.DiscountId.Value];
                    order.Discount = new Discount
                    {
                        Id = disc.DiscountId,
                        Name = disc.DiscountName ?? "N/A",
                        MaxCap = decimal.ToDouble(disc.DiscountCapEnd),
                        MinCap = decimal.ToDouble(disc.DiscountCapStart),
                        Type = "Percentage",
                        Value = disc.DiscountPercent,
                    };
                }

                var phoneId = dbOrder.PhoneId;
                var customerPhone = await dbContext.CustomerPhones.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();
                if (customerPhone != null)
                {
                    var customer = await dbContext.Customers.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();
                    var addressDetails = await dbContext.CustomerAddressDetails.Where(x => x.PhoneId == phoneId).FirstOrDefaultAsync();

                    var customerDetail = new CustomerDetail
                    {
                        FullName = customer?.CustomerName ?? "N/A",
                        MobileNumber = customerPhone.PhoneNumber ?? "N/A",
                        DeliveryAddress = addressDetails?.CompleteAddress ?? "N/A",
                        NearestLandmark = addressDetails?.LandMark ?? "N/A",
                        DeliveryInstructions = addressDetails?.Remarks ?? "N/A"
                    };
                    order.CustomerDetails = customerDetail;
                }
                await foreach (var item in GetOrderItemsAsync(dbContext, dbOrder.OrderMasterId, productDetails, dealItems, products, flavours, sizes, dealDescriptions))
                {
                    item.Price = item.Variations.Sum(x => x.Price + x.ItemChoices.SelectMany(y => y.ItemOptions).Sum(z => z.Price));
                    order.Items.Add(item);
                }

                yield return order;
            }
        }
    }

    internal async Task<Dictionary<int, string>> GetOrderStatusesAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        return await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
    }

    private async IAsyncEnumerable<MenuItem> GetOrderItemsAsync(Db.PgDbContext dbContext, int orderMasterId, List<Db.ProductDetail> productDetails, List<Db.DealItemDetail> dealItems, List<Db.Product> products, Dictionary<int, Db.Flavour> flavours, Dictionary<int, Db.ProductSize> sizes, List<Db.DealDescription> dealDescriptions)
    {
        var orderDetails = await dbContext.OrderDetails
            .Where(x => x.OrderMasterId == orderMasterId && x.IsActive == true)
            .ToListAsync() ?? [];
        var categories = await (from x in dbContext.ProductCategories
                                join y in dbContext.Products on x.CategoryId equals y.ProductCategoryId
                                select new { y.ProductId, x.CategoryId }).ToDictionaryAsync(x => x.ProductId, x => x.CategoryId);

        foreach (var orderDetail in orderDetails.Where(x => !x.DealItemId.HasValue))
        {
            var productDetail = productDetails.Where(x => x.ProductDetailId == orderDetail.ProductDetailId).First();
            var product = products.Where(x => x.ProductId == productDetail.ProductId).First();
            var flavour = flavours[productDetail.FlavourId!.Value];
            var size = sizes[productDetail.SizeId];
            var dealItemIdList = orderDetails
                        .Where(x => x.OrderParentId == orderDetail.ProductDetailId)
                        .Select(x => x.DealItemId)
                        .Distinct();

            yield return new MenuItem
            {
                Id = product.ProductId,
                Name = product.ProductName ?? "N/A",
                Image = product.ProductImage ?? "N/A",
                CategoryId = categories[product.ProductId].ToString(),
                IsKot = orderDetail.IsKot,
                Quantity = orderDetail.Quantity ?? 0,
                Variations =
                [
                    new ()
                        {
                            Id = productDetail.ProductDetailId,
                            Size = new ItemSize
                            {
                                Id = size.SizeId,
                                Name = size.SizeName ?? "N/A",
                            },
                            Flavour = new ItemFlavour
                            {
                                Id = flavour.FlavourId,
                                Name = flavour.FlavourName ?? "N/A",
                            },
                            Price = productDetail.Price,
                            ItemChoices = [.. dealItems.Where(x => dealItemIdList.Contains(x.DealItemId))

                            .Select(x => new ItemChoice{
                                Id  = x.DealItemId,
                                MaxChoice = x.MaxQuantity ?? 0,
                                Quantity = x.Quantity ?? 0,
                                Name = x.DealOptionName,
                                ItemOptions = [..orderDetails
                                .Where(y => y.OrderParentId == orderDetail.ProductDetailId && y.DealItemId == x.DealItemId)
                                .Select(y => new ItemOption {
                                    Id = y.ProductDetailId,
                                    Name = (from a in productDetails
                                            join b in products on a.ProductId equals b.ProductId
                                            where a.ProductDetailId == y.ProductDetailId
                                            select b.ProductName).FirstOrDefault() ?? "",
                                    Price = (from a in dealDescriptions
                                             where a.ProductDetailId == y.ProductDetailId && a.DealItemId == x.DealItemId
                                             select a.Price).FirstOrDefault() ?? 0.0,
                                    Quantity = (int?)(y.Quantity ?? 0.00)
                                })]
                            })],
                        }
                ]
            };
        }
    }

    internal async Task<object> GetRidersAsync(int userId, string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var list = await dbContext.Riders
            .Join(dbContext.UserBranchMappings, a => a.BranchId, b => b.BranchId, (a, b) => new { Riders = a, b.UserId })
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                Id = x.Riders.RiderId,
                Name = x.Riders.RiderName
            })
            .ToListAsync();
        return list;
    }

    internal async Task<object> GetBranchesAsync(string connectionString)
    {
        using var dbContext = GetDbContext(connectionString);
        var list = await dbContext.BranchMasters
            .Where(x => x.IsActive)
            .Select(x => new
            {
                Id = x.BranchId,
                Name = x.BranchName
            })
            .ToListAsync();
        return list;
    }
}