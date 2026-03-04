using Microsoft.EntityFrameworkCore;
using Npgsql;
using PointofSaleModels.Application;
using Db = PointofSaleModels.PGDatabaseModels;
using ValueType = PointofSaleModels.Application.ValueType;

namespace CreateOrderService;

public class Implementation()
{
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

    internal async Task<string> SaveOrderAsync(string connectionString, int branchId, CustomerOrder order)
    {
        var dbContext = GetDbContext(connectionString);
        order.BranchName = (await dbContext.BranchMasters.FirstOrDefaultAsync(x => x.BranchId == order.BranchId))?.BranchName ?? string.Empty;
        var orderMaster = await GetOrderMasterAsync(dbContext, branchId, order);
        await SetOnlineOrder(dbContext, branchId, orderMaster, order);
        order.OrderNumber = await SaveOrderAsync(dbContext, orderMaster);
        return order.OrderNumber;
    }

    private async Task<string> SaveOrderAsync(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        await dbContext.OrderMasters.AddAsync(orderMaster);
        await dbContext.SaveChangesAsync();
        await AssignOrderToken(dbContext, orderMaster);
        await AddOrderStatusLog(dbContext, orderMaster);
        return orderMaster.OrderToken;
    }

    private async Task AssignOrderToken(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        var orderToken = await GetUniqueToken(dbContext);
        orderMaster.OrderToken = orderToken;
        await dbContext.SaveChangesAsync();
    }

    private async Task<string> GetUniqueToken(Db.PgDbContext dbContext)
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
            return await GetUniqueToken(dbContext);
        }
    }

    private async Task AddOrderStatusLog(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        dbContext.OrderStatusLogs.Add(new Db.OrderStatusLog
        {
            CompanyId = orderMaster.CompanyId,
            OrderMasterId = orderMaster.OrderMasterId,
            OrderStatusId = orderMaster.OrderStatusId,
            CreatedDateTime = DateTime.UtcNow,
            Description = string.Empty,
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<string> GenerateOrderNumberAsync(Db.PgDbContext dbContext, int branchId)
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

    private async Task<Db.OrderMaster> GetOrderMasterAsync(Db.PgDbContext dbContext, int branchId, CustomerOrder order)
    {
        var companyId = await dbContext.SetupCompanies.Select(x => x.CompanyId).FirstAsync();
        var discount = order.Discount;
        var orderNumber = await GenerateOrderNumberAsync(dbContext, branchId);
        var subTotal = order.Items.Select(x => x.Variations.Select(x => x.Price).Sum()).Sum();
        var dbPaymentMode = await dbContext.PaymentModes.FirstOrDefaultAsync(x => x.PaymentMode1 == order.PaymentType);
        var gst = dbPaymentMode != null ? await dbContext.Gsts.FirstOrDefaultAsync(x => x.PaymentModeId == dbPaymentMode.PaymentModeId) : null;
        var tax = gst?.Gstpercentage ?? 0.00;
        var orderSourceId = await dbContext.SetupMasterDetails.Where(x => x.CompanyId == companyId && x.Flex1 == "WEB").Select(x => x.SetupDetailId).FirstOrDefaultAsync();
        var orderstatus = await dbContext.OrderStatuses.Where(x => x.OrderStatusName == "Pending").FirstOrDefaultAsync();
        order.Status = OrderStatus.Pending.ToString();
        var orderType = await dbContext.SetupMasterDetails.FirstOrDefaultAsync(x => x.SetupDetailName == order.OrderType);
        order.OrderType = orderType.SetupDetailName;
        var branchDetail = await dbContext.BranchDetails.FirstOrDefaultAsync(x => x.BranchId == branchId);
        var areaId = branchDetail?.AreaId;
        var deliveryTime = branchDetail?.DeliveryTime ?? 0;
        var orderMaster = new Db.OrderMaster
        {
            OrderSourceId = orderSourceId,
            OrderStatusId = orderstatus.OrderStatusId,
            OrderNumber = orderNumber,
            CompanyId = companyId,
            BranchId = branchId,
            AreaId = areaId,
            OrderModeId = orderType.SetupDetailId,
            OrderDate = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()),
            OrderTime = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime()),
            TotalAmountWithoutGst = subTotal,
            TotalAmountWithGst = subTotal + (subTotal * tax / 100),
            DiscountAmount = discount?.Type == ValueType.Amount.ToString() ? discount.Value : 0.00,
            DiscountId = discount?.Id ?? 0,
            DiscountPercent = discount?.Type == ValueType.Percentage.ToString() ? discount.Value : 0.00,
            Gstamount = subTotal * (tax / 100),
            Gstid = gst?.Gstid,
            Gstpercent = tax,
            IsActive = true,
            SpecialInstruction = order.Description,
            OrderDetails = [],
            AlternateNumber = order.CustomerDetails.AlternateMobileNumber ?? string.Empty,
            DeliveryCharges = order.DeliveryCharges ?? 0.00,
            DeliveryTime = deliveryTime,
        };
        order.AmountWithGst = orderMaster.TotalAmountWithGst ?? 0.0;
        order.AmountWithoutGst = orderMaster.TotalAmountWithoutGst ?? 0.0;
        foreach (var orderDetail in GetOrderDetails(order.Items, gst))
        {
            orderMaster.OrderDetails.Add(orderDetail);
        }
        return orderMaster;
    }

    private List<Db.OrderDetail> GetOrderDetails(List<MenuItem> items, Db.Gst? gst = null)
    {
        var list = new List<Db.OrderDetail>();
        foreach (var item in items)
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
                        list.Add(new Db.OrderDetail
                        {
                            RandomId = orderDetail.RandomId,
                            OrderParentId = variation.Id,
                            DealItemId = choice.Id,
                            ProductDetailId = option.Id,
                            Quantity = option.Quantity.HasValue ? option.Quantity : choice.Quantity,
                            IsKot = true,
                            IsActive = true,
                            Gstid = gst?.Gstid,
                            PriceWithGst = option.Price + (option.Price * (gst?.Gstpercentage ?? 0) / 100),
                            PriceWithoutGst = option.Price,
                            DiscountId = variation.Discount.Id,
                            DiscountPercent = variation.Discount.Value,
                        });
                    }
                }
            }
            if (gst != null)
            {
                orderDetail.Gstid = gst.Gstid;
                orderDetail.PriceWithGst = variation?.Price;
                orderDetail.PriceWithoutGst = variation != null ? variation.Price / (1 + (gst.Gstpercentage / 100)) : 0.00;

            }
            list.Add(orderDetail);
        }
        return list;
    }

    private async Task SetOnlineOrder(Db.PgDbContext dbContext, int branchId, Db.OrderMaster orderMaster, CustomerOrder order)
    {
        var cd = order.CustomerDetails;
        var add = cd.DeliveryAddress ?? string.Empty;
        var customer = new Customer
        {
            Contact = cd.MobileNumber ?? string.Empty,
            Addresses = [add],
            Name = cd.FullName ?? string.Empty,
            SelectedAddress = add,
        };
        if (customer.Addresses == null || customer.Addresses.Count == 0)
        {
            throw new Exception("Customer must have at least one address");
        }
        var companyId = orderMaster.CompanyId;

        var dbCustomerPhone = await SaveCustomerPhoneAsync(dbContext, companyId, customer);
        orderMaster.PhoneId = dbCustomerPhone.PhoneId;

        var dbCustomer = await dbContext.Customers
            .Where(t => t.CustomerName != null && customer.Name != null && t.CustomerName.Trim().ToLower().Equals(customer.Name.Trim().ToLower()))
            .FirstOrDefaultAsync();

        if (dbCustomer == null)
        {
            dbCustomer = new Db.Customer
            {
                Title = cd.Title,
                CustomerName = customer.Name,
                CompanyId = companyId,
                CustomerPhone = dbCustomerPhone,
            };
            await dbContext.Customers.AddAsync(dbCustomer);
            await dbContext.SaveChangesAsync();
        }
        orderMaster.CustomerId = dbCustomer.CustomerId;

        var firstAddress = customer.Addresses.First().Trim();
        var dbCustomerAddress = dbContext.CustomerAddressDetails
            .Where(t => t.CompleteAddress != null && t.CompleteAddress.Trim().ToLower().Equals(firstAddress.ToLower()))
            .FirstOrDefault();

        if (dbCustomerAddress == null)
        {
            var cityId = (await dbContext.Areas.FirstAsync(x => x.AreaId == orderMaster.AreaId.Value))?.CityId;
            dbCustomerAddress = new Db.CustomerAddressDetail
            {
                CustomerPhone = dbCustomerPhone,
                CompanyId = companyId,
                CompleteAddress = firstAddress,
                CityId = cityId.Value,
                AreaId = orderMaster.AreaId.Value,
            };
            dbContext.CustomerAddressDetails.Add(dbCustomerAddress);
            await dbContext.SaveChangesAsync();
        }
        orderMaster.CustomerAddressId = dbCustomerAddress.CustomerAddressId;
        // orderMaster.RiderId = order.Rider?.Id;
        // orderMaster.DeliveryCharges = order.DeliveryCharges?.Value;
    }

    private async Task<Db.CustomerPhone> SaveCustomerPhoneAsync(Db.PgDbContext dbContext, int companyId, Customer customer)
    {
        Db.CustomerPhone? dbCustomerPhone;
        var cust = customer ?? throw new Exception("Customer is required");
        if (cust.PhoneId == 0)
        {
            dbCustomerPhone = await dbContext.CustomerPhones
            .Where(t => t.PhoneNumber != null && cust.Contact != null && t.PhoneNumber.Trim().Equals(cust.Contact.Trim()))
            .FirstOrDefaultAsync();
            if (dbCustomerPhone == null)
            {
                dbCustomerPhone = new Db.CustomerPhone
                {
                    PhoneNumber = cust.Contact ?? string.Empty,
                    CompanyId = companyId,
                    IsActive = true,
                };

                await dbContext.CustomerPhones.AddAsync(dbCustomerPhone);
                await dbContext.SaveChangesAsync();
            }
        }
        else
        {
            dbCustomerPhone = await dbContext.CustomerPhones
                                   .Where(x => x.PhoneId == cust.PhoneId)
                                   .FirstOrDefaultAsync();
        }
        if (dbCustomerPhone == null)
        {
            throw new Exception("Customer phone record could not be resolved");
        }
        return dbCustomerPhone;
    }

    internal async IAsyncEnumerable<int> GetBranchUsersIdsAsync(string connectionString, int branchId)
    {
        using var dbContext = GetDbContext(connectionString);
        foreach (var userId in await dbContext.UserBranchMappings.Where(x => x.BranchId == branchId).Select(x => x.UserId).ToListAsync())
            yield return userId;
    }

    internal async Task<object> OrderStatusLogs(string connectionString, string orderToken)
    {
        var karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
        var dbContext = GetDbContext(connectionString);
        var orderMasterId = await dbContext.OrderMasters.Where(x => x.OrderToken == orderToken).Select(x => x.OrderMasterId).FirstOrDefaultAsync();
        var logs = await dbContext.OrderStatusLogs.Where(x => x.OrderMasterId == orderMasterId).ToListAsync();
        return logs.Select(x => new
        {
            Id = x.OrderStatusId,
            CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.SpecifyKind(x.CreatedDateTime, DateTimeKind.Utc),
                            karachiTz
                        ),
        });
    }
}