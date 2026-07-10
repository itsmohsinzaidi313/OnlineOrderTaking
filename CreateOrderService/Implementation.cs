using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using PointofSaleModels.Application;
using PointofSaleModels.Services;
using System.Text.Json.Nodes;
using Db = PointofSaleModels.PGDatabaseModels;
using ValueType = PointofSaleModels.Application.ValueType;

namespace CreateOrderService;

public class Implementation(IRestaurantDbContextFactory restaurantDbContextFactory, IDbContextFactory<Db.RestaurantsContext> dbContextFactory)
{
    internal async Task SaveToken(string url, string orderToken)
    {
        await using var context = dbContextFactory.CreateDbContext();
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == url);
        var restaurantId = restaurant?.Id ?? throw new Exception("Restaurant not found");
        var tokenEntity = new Db.OrderTokens { OrderToken = orderToken, CreatedAt = DateTime.UtcNow, RestaurantId = restaurantId };
        await context.OrderTokens.AddAsync(tokenEntity);
        await context.SaveChangesAsync();
    }

    internal async Task SaveOrderAsync(string url, CustomerOrder order)
    {
        var branchId = order.BranchId;
        var areaId = order.AreaId;
        await using var dbContext = await restaurantDbContextFactory.CreateDbContextAsync(url, readOnly: false);
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                order.BranchName = (await dbContext.BranchMasters.FirstOrDefaultAsync(x => x.BranchId == order.BranchId))?.BranchName ?? string.Empty;
                if (areaId.HasValue)
                {
                    var areacity = await dbContext.Areas.Where(x => x.AreaId == areaId).Join(dbContext.Cities, a => a.CityId, b => b.CityId, (a, b) => new { a.AreaName, b.CityName }).FirstOrDefaultAsync();
                    order.AreaName = areacity.AreaName;
                    order.CityName = areacity.CityName;
                }
                var orderMaster = await GetOrderMasterAsync(dbContext, order);
                await SetOnlineOrder(dbContext, branchId, orderMaster, order);
                order.OrderToken = await SaveOrderAsync(dbContext, orderMaster);
                order.OrderNumber = orderMaster.OrderNumber;
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private static async Task<string> SaveOrderAsync(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        await dbContext.OrderMasters.AddAsync(orderMaster);
        await dbContext.SaveChangesAsync();
        await AddOrderStatusLog(dbContext, orderMaster);
        return orderMaster.OrderToken;
    }

    private static async Task<string> GetUniqueTokenAsync(Db.PgDbContext dbContext)
    {
        var token = TokenGenerator.GenerateToken();
        var existingToken = await dbContext.OrderMasters
            .FirstOrDefaultAsync(x => x.OrderToken == token);
        if (existingToken == null)
        {
            return token;
        }
        else
        {
            var newToken = TokenGenerator.GenerateToken();
            return await GetUniqueTokenAsync(dbContext);
        }
    }

    private static async Task AddOrderStatusLog(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        dbContext.OrderStatusLogs.Add(new Db.OrderStatusLog
        {
            CompanyId = orderMaster.CompanyId,
            OrderMasterId = orderMaster.OrderMasterId,
            OrderStatusId = orderMaster.OrderStatusId,
            CreatedDate = DateTime.UtcNow,
            Description = string.Empty,
        });
        await dbContext.SaveChangesAsync();
    }

    public static async Task<string> GenerateOrderNumberAsync(Db.PgDbContext dbContext, int branchId)
    {
        var id = await dbContext.Database.SqlQuery<long>($"""
                                                            INSERT INTO branch_order_sequence ("BranchId", "LastValue")
                                                            VALUES ({branchId}, 1)
                                                            ON CONFLICT ("BranchId")
                                                            DO UPDATE
                                                                SET "LastValue" = branch_order_sequence."LastValue" + 1 WHERE branch_order_sequence."BranchId" = {branchId}
                                                            RETURNING "LastValue"
                                                        """).ToListAsync();

        var now = DateTime.Now;
        var datePrefix = now.ToString("yyyyMMdd");
        var prefix = $"{datePrefix}/ORD/";
        var orderNumber = $"{prefix}{id.First():D5}";
        return orderNumber;
    }

    private static async Task<Db.OrderMaster> GetOrderMasterAsync(Db.PgDbContext dbContext, CustomerOrder order)
    {
        var branchId = order.BranchId;
        var areaId = order.AreaId;
        var companyId = await dbContext.SetupCompanies.Select(x => x.CompanyId).FirstAsync();
        var orderNumber = await GenerateOrderNumberAsync(dbContext, branchId);
        var dbPaymentMode = await dbContext.PaymentModes.FirstOrDefaultAsync(x => x.PaymentMode1.ToLower() == order.PaymentType.ToLower());
        var gst = dbPaymentMode != null ? await dbContext.Gsts.FirstOrDefaultAsync(x => x.PaymentModeId == dbPaymentMode.PaymentModeId) : null;
        var orderSource = await dbContext.SetupMasterDetails.Where(x => x.CompanyId == companyId && x.Flex1 == "WEB").FirstOrDefaultAsync();
        var orderstatus = await dbContext.OrderStatuses.Where(x => x.OrderStatusName == "Pending").FirstOrDefaultAsync();
        order.Status = OrderStatus.Pending.ToString();
        var orderType = await dbContext.SetupMasterDetails.FirstOrDefaultAsync(x => x.SetupDetailName == order.OrderType);
        order.OrderType = orderType.SetupDetailName;
        order.GstPercentage = gst?.Gstpercentage ?? 0.00;

        var orderMaster = new Db.OrderMaster
        {
            OrderSourceId = orderSource.SetupDetailId,
            OrderStatusId = orderstatus!.OrderStatusId,
            OrderSourceValue = orderSource.Flex1,
            OrderNumber = orderNumber,
            CompanyId = companyId,
            BranchId = branchId,
            OrderModeId = orderType.SetupDetailId,
            OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
            OrderTime = TimeOnly.FromDateTime(DateTime.UtcNow),
            Gstid = gst?.Gstid,
            Gstpercent = gst?.Gstpercentage ?? 0.00,
            IsActive = true,
            OrderDetails = [],
            AlternateNumber = order.CustomerDetails.AlternateMobileNumber ?? string.Empty,
            TotalAmountWithGst = 0.00,
            TotalAmountWithoutGst = 0.00,
            Gstamount = 0.00,
            DiscountAmount = 0.00,
            OrderToken = await GetUniqueTokenAsync(dbContext),
            ChangeAmount = order.CustomerDetails.ChangeAmount,
            Exported = false,
            PaymentTypeId = dbPaymentMode.PaymentModeId
        };

        if (areaId.HasValue)
        {
            var branchDetail = await dbContext.BranchDetails.FirstOrDefaultAsync(x => x.AreaId == areaId.Value && x.BranchId == branchId);
            if (branchDetail != null)
            {
                orderMaster.AreaId = areaId;
                orderMaster.DeliveryCharges = branchDetail.DeliveryCharges;
                orderMaster.DeliveryTime = branchDetail.DeliveryTime;
            }
        }

        foreach (var item in order.Items)
        {
            foreach (var orderDetail in GetOrderDetails(item, gst))
            {
                var itemPrice = orderDetail.PriceWithoutGst ?? 0.00;
                var itemQuantity = orderDetail.Quantity ?? 0;
                var discountPercent = orderDetail.DiscountPercent;

                var totalItemPrice = itemPrice * itemQuantity;

                var itemDiscount = 0.00;

                if (discountPercent.HasValue)
                {
                    itemDiscount = totalItemPrice * ((discountPercent ?? 0.00) / 100);
                }
                orderMaster.TotalAmountWithoutGst += totalItemPrice;
                orderMaster.DiscountAmount += double.Round(itemDiscount, MidpointRounding.ToZero);
                orderMaster.OrderDetails.Add(orderDetail);
            }
        }
        orderMaster.TotalAmountWithGst = orderMaster.TotalAmountWithoutGst + (orderMaster.TotalAmountWithoutGst * (gst?.Gstpercentage ?? 0) / 100);
        orderMaster.Gstamount = orderMaster.TotalAmountWithGst - orderMaster.TotalAmountWithoutGst;
        order.AmountWithGst = orderMaster.TotalAmountWithGst ?? 0.0;
        order.AmountWithoutGst = orderMaster.TotalAmountWithoutGst ?? 0.0;
        return orderMaster;
    }

    private static IEnumerable<Db.OrderDetail> GetOrderDetails(MenuItem item, Db.Gst? gst = null)
    {
        var orderDetail = new Db.OrderDetail
        {
            IsKot = true,
            IsActive = true,
            SpecialInstruction = item.Comment,
            Quantity = item.Quantity,
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
                    yield return new Db.OrderDetail
                    {
                        OrderParentId = variation.Id,
                        RandomId = orderDetail.RandomId,
                        DealItemId = choice.Id,
                        ProductDetailId = option.Id,
                        Quantity = option.Quantity.HasValue ? (option.Quantity * item.Quantity) : choice.Quantity,
                        IsKot = true,
                        IsActive = true,
                        Gstid = gst?.Gstid,
                        PriceWithGst = double.Round(option.Price + (option.Price * (gst?.Gstpercentage ?? 0) / 100), MidpointRounding.ToZero),
                        PriceWithoutGst = option.Price,
                    };
                }
            }
        }
        orderDetail.Gstid = gst?.Gstid;
        orderDetail.DiscountId = variation?.Discount?.Id;
        orderDetail.DiscountPercent = variation?.Discount?.Value;
        orderDetail.IsPercentage = variation?.Discount?.Type == ValueType.Percentage.ToString();
        orderDetail.PriceWithoutGst = variation?.Price;
        var itemPrice = variation?.Price ?? 0.00;
        orderDetail.PriceWithGst = double.Round(itemPrice + (itemPrice * (gst?.Gstpercentage ?? 0) / 100), MidpointRounding.ToZero);
        yield return orderDetail;
    }

    private static async Task SetOnlineOrder(Db.PgDbContext dbContext, int branchId, Db.OrderMaster orderMaster, CustomerOrder order)
    {
        var cd = order.CustomerDetails;
        if (cd == null)
        {
            throw new Exception("Customer details are required for online orders");
        }
        var companyId = orderMaster.CompanyId;
        var dbCustomerPhone = await SaveCustomerPhoneAsync(dbContext, companyId, cd);
        orderMaster.PhoneId = dbCustomerPhone.PhoneId;

        var dbCustomer = await dbContext.Customers
            .Where(x => x.PhoneId == dbCustomerPhone.PhoneId)
            .FirstOrDefaultAsync();

        if (dbCustomer == null)
        {
            dbCustomer = new Db.Customer
            {
                Title = cd.Title,
                CustomerName = cd.FullName,
                CompanyId = companyId,
                CustomerPhone = dbCustomerPhone,
                Email = cd.EmailAddress ?? string.Empty,
            };
            await dbContext.Customers.AddAsync(dbCustomer);
        }
        else
        {
            dbCustomer.Title = cd.Title;
            dbCustomer.CustomerName = cd.FullName;
            dbCustomer.Email = cd.EmailAddress ?? string.Empty;
            dbContext.Customers.Update(dbCustomer);
        }
        await dbContext.SaveChangesAsync();
        orderMaster.CustomerId = dbCustomer.CustomerId;

        var firstAddress = cd.DeliveryAddress?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(cd.DeliveryAddress))
        {

            var dbCustomerAddress = dbContext.CustomerAddressDetails
                .Where(x => x.CompleteAddress == firstAddress)
                .FirstOrDefault();

            if (dbCustomerAddress == null)
            {
                var cityId = (await dbContext.Areas.FirstAsync(x => x.AreaId == orderMaster.AreaId!.Value))?.CityId;
                dbCustomerAddress = new Db.CustomerAddressDetail
                {
                    CustomerPhone = dbCustomerPhone,
                    CompanyId = companyId,
                    CompleteAddress = firstAddress,
                    CityId = cityId!.Value,
                    AreaId = orderMaster.AreaId!.Value,
                    LandMark = cd.NearestLandmark ?? string.Empty,
                };
                dbContext.CustomerAddressDetails.Add(dbCustomerAddress);
            }
            else
            {
                dbCustomerAddress.CustomerPhone = dbCustomerPhone;
                dbCustomerAddress.CompleteAddress = firstAddress;
                dbCustomerAddress.CityId = (await dbContext.Areas.FirstAsync(x => x.AreaId == orderMaster.AreaId!.Value))?.CityId ?? 0;
                dbCustomerAddress.AreaId = orderMaster.AreaId!.Value;
                dbCustomerAddress.LandMark = cd.NearestLandmark ?? string.Empty;
                dbContext.CustomerAddressDetails.Update(dbCustomerAddress);
            }
            await dbContext.SaveChangesAsync();
            orderMaster.CustomerAddressId = dbCustomerAddress.CustomerAddressId;
        }
        orderMaster.SpecialInstruction = cd.DeliveryInstructions;
        orderMaster.EmailAddress = cd.EmailAddress;
        orderMaster.AlternateNumber = cd.AlternateMobileNumber;
    }

    private static async Task<Db.CustomerPhone> SaveCustomerPhoneAsync(Db.PgDbContext dbContext, int companyId, CustomerDetail customer)
    {
        Db.CustomerPhone? dbCustomerPhone;
        dbCustomerPhone = await dbContext.CustomerPhones
        .FirstOrDefaultAsync(x => x.PhoneNumber == customer.MobileNumber);
        if (dbCustomerPhone == null)
        {
            dbCustomerPhone = new Db.CustomerPhone
            {
                PhoneNumber = customer.MobileNumber ?? string.Empty,
                CompanyId = companyId,
                IsActive = true,
            };

            await dbContext.CustomerPhones.AddAsync(dbCustomerPhone);
            await dbContext.SaveChangesAsync();
        }
        return dbCustomerPhone;
    }

    internal async IAsyncEnumerable<int> GetBranchUsersIdsAsync(string url, int branchId)
    {
        await using var dbContext = await restaurantDbContextFactory.CreateDbContextAsync(url);
        foreach (var userId in await dbContext.UserBranchMappings.Where(x => x.BranchId == branchId).Select(x => x.UserId).ToListAsync())
            yield return userId;
    }

    internal async Task<object> OrderStatusLogs(string url, string orderToken)
    {
        var karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
        await using var dbContext = await restaurantDbContextFactory.CreateDbContextAsync(url);
        var orderMasterId = await dbContext.OrderMasters.Where(x => x.OrderToken == orderToken).Select(x => x.OrderMasterId).FirstOrDefaultAsync();
        var logs = await dbContext.OrderStatusLogs.Where(x => x.OrderMasterId == orderMasterId).ToListAsync();
        return logs.Select(x => new
        {
            Id = x.OrderStatusId,
            CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.SpecifyKind(x.CreatedDate, DateTimeKind.Utc),
                            karachiTz
                        ),
        });
    }

    internal async Task<JsonObject> GetLegacyResponse(string orderNumber, string url)
    {
        await using var dbContext = await restaurantDbContextFactory.CreateDbContextAsync(url);
        var om = await dbContext.OrderMasters
            .FirstOrDefaultAsync(x => x.OrderToken == orderNumber);

        var orderDetails = await dbContext.OrderDetails
            .Where(x => x.OrderMasterId == om!.OrderMasterId)
            .ToListAsync();
        var branchName = await dbContext.BranchMasters.Where(x => x.BranchId == om.BranchId).Select(x => x.BranchName).FirstOrDefaultAsync();

        var categories = await dbContext.ProductCategories.ToDictionaryAsync(x => x.CategoryId, x => x.CategoryName);
        var dbProducts = await dbContext.Products.ToListAsync();
        var products = dbProducts.ToDictionary(x => x.ProductId, x => x.ProductName);
        var productImages = dbProducts.ToDictionary(x => x.ProductId, x => x.ProductImage);
        var productCategoryIds = dbProducts.ToDictionary(x => x.ProductId, x => x.ProductCategoryId);
        var productDetails = await dbContext.ProductDetails.ToDictionaryAsync(x => x.ProductDetailId, x => x.ProductId);
        var orderMaster = new JsonObject
        {
            ["HasError"] = 0,
            ["Error_Message"] = "",
            ["Message"] = "Your Order Placed Successfully.",
            ["Id"] = om.OrderMasterId,
            ["OrderNumber"] = om.OrderNumber,
            ["OrderDate"] = JsonValue.Create(om.OrderDate),
            ["SrbInvoiceId"] = om.SrbInvoiceId,
            ["FbrInvoiceId"] = om.FbrInvoiceId,
            ["IsThirdPartyPaymentIntegration"] = 0,
            ["OrderStatusId"] = om.OrderStatusId,
            ["CompanyId"] = om.CompanyId,
            ["BranchId"] = om.BranchId,
            ["BranchName"] = branchName,
            ["AdditionalMsg"] = om.Remarks,
            ["SubTotal"] = om.TotalAmountWithoutGst,
            ["DeliveryCharges"] = om.DeliveryCharges,
            ["Discount"] = om.DiscountId,
            ["GstAmount"] = om.Gstamount,
            ["GstPercent"] = om.Gstpercent,
            ["NetBill"] = om.TotalAmountWithGst
        };
        JsonArray masters = new JsonArray() { orderMaster };
        JsonArray details = new JsonArray();
        foreach (var od in orderDetails)
        {
            var productId = productDetails[od.ProductDetailId];
            var productName = products[productId];
            var productImage = productImages[productId];
            var categoryName = productCategoryIds[productId];
            var y = new JsonObject
            {
                ["OrderDetailId"] = od.OrderDetailId,
                ["ProductDetailId"] = od.ProductDetailId,
                ["DealItemId"] = od.DealItemId,
                ["RandomId"] = od.RandomId,
                ["Quantity"] = od.Quantity,
                ["ProductName"] = productName,
                ["CategoryName"] = categoryName,
                ["AmountWithoutGST"] = od.PriceWithoutGst,
                ["AmountWithGST"] = od.PriceWithGst,
                ["ProductImage"] = productImage
            };
            details.Add(y);
        }
        var dataSet = new JsonObject
        {
            ["Table"] = masters,
            ["Table1"] = details
        };
        string? data = null;
        return new JsonObject
        {
            ["Response"] = true,
            ["ResponseCodes"] = "00",
            ["ResponseMessage"] = "Success",
            ["Data"] = data,
            ["DataSet"] = dataSet
        };
    }
}