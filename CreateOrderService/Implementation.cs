using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using Db = PointofSaleModels.PGDatabaseModels;
using ValueType = PointofSaleModels.Application.ValueType;

namespace CreateOrderService;

class Implementation()
{
    const int ORDER_STATUS_PENDING = 802;
    const int SOURCE_ID = 532;
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
    internal async Task<string> SaveOrderAsync(string connectionString, int branchId, CustomerOrder order)
    {
        var dbContext = GetDbContext(connectionString);
        var orderMaster = await GetOrderMasterAsync(dbContext, branchId, order);
        await SetOnlineOrder(dbContext, branchId, orderMaster, order);
        return await SaveOrderAsync(dbContext, orderMaster);
    }

    private static async Task<string> SaveOrderAsync(Db.PgDbContext dbContext, Db.OrderMaster orderMaster)
    {
        await dbContext.OrderMasters.AddAsync(orderMaster);
        await dbContext.SaveChangesAsync();
        return orderMaster.OrderNumber;
    }

    public static async Task<string> GenerateOrderNumberAsync(Db.PgDbContext dbContext, int branchId)
    {
        var id = await dbContext.Database.SqlQuery<long>($"""
                                                            INSERT INTO branch_order_sequence ("BranchId", "LastValue")
                                                            VALUES ({branchId}, 1)
                                                            ON CONFLICT ("BranchId")
                                                            DO UPDATE
                                                                SET "LastValue" = branch_order_sequence."LastValue" + 1
                                                            RETURNING "LastValue"
                                                        """).ToListAsync();

        var now = DateTime.UtcNow;
        var datePrefix = now.ToString("ddMMyy");
        var prefix = $"{datePrefix}/ORD/";
        var orderNumber = $"{prefix}{id.First():D4}";
        return orderNumber;
    }

    private static async Task<int> GetOrderModeIdAsync(Db.PgDbContext dbContext, string orderMode, int companyId)
    {
        var setupMaster = await dbContext.SetupMasters.Where(x => x.SetupMasterName == "OrderMode").FirstAsync();
        return (await dbContext.SetupMasterDetails
                        .Where(x => x.SetupMasterId == setupMaster.SetupMasterId)
                        .FirstOrDefaultAsync())!.SetupDetailId;
    }

    private async Task<Db.OrderMaster> GetOrderMasterAsync(Db.PgDbContext dbContext, int branchId, CustomerOrder order)
    {
        var companyId = await dbContext.SetupCompanies.Select(x => x.CompanyId).FirstAsync();
        var orderSourceId = SOURCE_ID;
        var discount = order.Discount;
        var orderNumber = await GenerateOrderNumberAsync(dbContext, branchId);
        int orderStatusId = ORDER_STATUS_PENDING;
        var orderModeId = await GetOrderModeIdAsync(dbContext, order.OrderType.ToString(), companyId);
        var subTotal = order.Items.Select(x => x.Variations.Select(x => x.Price).Sum()).Sum();
        var gst = await GetTaxPercentageAsync(dbContext);
        var tax = gst?.Gstpercentage ?? 0.00;
        var amountWithTax = subTotal + (subTotal * tax / 100);

        var orderMaster = new Db.OrderMaster
        {
            OrderSourceId = orderSourceId,
            OrderNumber = orderNumber,
            CompanyId = companyId,
            BranchId = branchId,
            OrderStatusId = orderStatusId,
            OrderModeId = orderModeId,
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            OrderTime = TimeOnly.FromDateTime(DateTime.Now),
            TotalAmountWithoutGst = subTotal,
            TotalAmountWithGst = amountWithTax,
            DiscountAmount = discount?.Type == ValueType.Amount.ToString() ? discount.Value : 0.00,
            DiscountId = discount?.Id,
            DiscountPercent = discount?.Type == ValueType.Percentage.ToString() ? discount.Value : 0.00,
            Gstamount = subTotal * (tax / 100),
            Gstid = gst?.Gstid,
            Gstpercent = tax,
            IsActive = true,
            Cover = 0,
            SpecialInstruction = order.Description,
        };
        foreach (var orderDetail in GetOrderDetails(order.Items))
        {
            orderMaster.OrderDetails.Add(orderDetail);
        }
        return orderMaster;
    }

    private static List<Db.OrderDetail> GetOrderDetails(List<MenuItem> items)
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
                            Quantity = choice.Quantity,
                            IsKot = true,
                            IsActive = true,
                        });
                    }
                }
            }
            list.Add(orderDetail);
        }
        return list;
    }

    private async Task SetOnlineOrder(Db.PgDbContext dbContext, int branchId, Db.OrderMaster orderMaster, CustomerOrder order)
    {
        var cd = order.CustomerDetails;
        var add = cd.DeliveryAddress ?? string.Empty;
        order.Customer = new Customer
        {
            Contact = cd.MobileNumber ?? string.Empty,
            Addresses = [add],
            Name = cd.FullName ?? string.Empty,
            SelectedAddress = add,
        };
        var customer = order.Customer ?? throw new Exception("Invalid customer information");
        if (customer.Addresses == null || customer.Addresses.Count == 0)
        {
            throw new Exception("Customer must have at least one address");
        }
        var companyId = orderMaster.CompanyId;

        var dbCustomerPhone = await SaveCustomerPhoneAsync(dbContext, companyId, order);
        orderMaster.PhoneId = dbCustomerPhone.PhoneId;

        var dbCustomer = await dbContext.Customers
            .Where(t => t.CustomerName != null && customer.Name != null && t.CustomerName.Trim().ToLower().Equals(customer.Name.Trim().ToLower()))
            .FirstOrDefaultAsync();

        if (dbCustomer == null)
        {
            dbCustomer = new Db.Customer
            {
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

            var record = await GetCityId_AreaId_ByBranchIdAsync(dbContext, branchId);
            dbCustomerAddress = new Db.CustomerAddressDetail
            {
                CustomerPhone = dbCustomerPhone,
                CompanyId = companyId,
                CompleteAddress = firstAddress,
                CityId = record.Item1,
                AreaId = record.Item2,
            };
            dbContext.CustomerAddressDetails.Add(dbCustomerAddress);
            await dbContext.SaveChangesAsync();
        }
        orderMaster.CustomerAddressId = dbCustomerAddress.CustomerAddressId;
        // orderMaster.RiderId = order.Rider?.Id;
        // orderMaster.DeliveryCharges = order.DeliveryCharges?.Value;
    }

    internal async Task<(int, int)> GetCityId_AreaId_ByBranchIdAsync(Db.PgDbContext dbContext, int branchId)
    {
        int areaId = await dbContext.BranchDetails.Where(x => x.BranchId.Equals(branchId)).Select(x => x.AreaId).FirstOrDefaultAsync();
        int cityId = await dbContext.Areas.Where(x => x.AreaId.Equals(areaId)).Select(x => x.CityId).FirstOrDefaultAsync() ?? 0;
        return (cityId, areaId);
    }

    private static async Task<Db.CustomerPhone> SaveCustomerPhoneAsync(Db.PgDbContext dbContext, int companyId, CustomerOrder order)
    {
        Db.CustomerPhone? dbCustomerPhone;
        var cust = order.Customer ?? throw new Exception("Customer is required");
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

    private static async Task<Db.Gst?> GetTaxPercentageAsync(Db.PgDbContext dbContext)
    {
        return await dbContext.Gsts
            .FirstOrDefaultAsync();
    }
}