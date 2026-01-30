using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Application;
using Db = PointofSaleModels.PGDatabaseModels;
using ValueType = PointofSaleModels.Application.ValueType;

namespace CreateOrderService;

class Implementation(Db.PgDbContext dbContext)
{
    const int ORDER_STATUS_PENDING = 802;
    const int SOURCE_ID = 532;
    internal async Task<string> SaveOrderAsync(int companyId, int branchId, CustomerOrder order)
    {
        var orderMaster = await GetOrderMasterAsync(companyId, branchId, order);
        await SetOnlineOrder(branchId, orderMaster, order);
        return await SaveOrderAsync(orderMaster);
    }

    private async Task<string> SaveOrderAsync(Db.OrderMaster orderMaster)
    {
        await dbContext.OrderMasters.AddAsync(orderMaster);
        await dbContext.SaveChangesAsync();
        return orderMaster.OrderNumber;
    }

    public async Task<string> GenerateOrderNumberAsync(int branchId)
    {
        var id = await dbContext.Database.SqlQuery<long>($"""
                                            UPDATE branch_order_sequences
                                            SET last_value = last_value + 1
                                            WHERE branch_id = {branchId}
                                            RETURNING last_value
                                            """).SingleAsync();
        var now = DateTime.UtcNow;
        var datePrefix = now.ToString("ddMMyy");
        var prefix = $"{datePrefix}/ORD/";
        var orderNumber = $"{prefix}{id:D4}";
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
        var orderSourceId = SOURCE_ID;
        var discount = order.Discount;
        var orderNumber = order.OrderNumber ?? await GenerateOrderNumberAsync(branchId);
        if (OrderNumberExists(branchId, orderNumber))
        {
            orderNumber = await GenerateOrderNumberAsync(branchId);
        }
        int orderStatusId = ORDER_STATUS_PENDING;

        var orderModeId = GetOrderModeId(order.OrderType.ToString(), companyId);
        var subTotal = order.Items.Select(x => x.Variations.Select(x => x.Price).Sum()).Sum();

        var amountWithTax = subTotal + (subTotal * 0.00);

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
            Gstamount = subTotal * (tax?.Value / 100) ?? 0.00,
            Gstid = tax?.Id,
            Gstpercent = tax?.Value ?? 0.00,
            IsActive = true,
            Cover = order.Persons,
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

    private async Task SetOnlineOrder(int branchId, Db.OrderMaster orderMaster, CustomerOrder order)
    {
        var customer = order.Customer ?? throw new Exception("Invalid customer information");
        if (customer.Addresses == null || customer.Addresses.Count == 0)
        {
            throw new Exception("Customer must have at least one address");
        }
        var companyId = orderMaster.CompanyId;

        var dbCustomerPhone = await SaveCustomerPhone(companyId, order);
        orderMaster.PhoneId = dbCustomerPhone.PhoneId;

        var dbCustomer = dbContext.Customers
            .Where(t => t.CustomerName != null && customer.Name != null && t.CustomerName.Trim().ToLower().Equals(customer.Name.Trim().ToLower()))
            .FirstOrDefault();

        if (dbCustomer == null)
        {
            dbCustomer = new Db.Customer
            {
                CustomerName = customer.Name,
                CompanyId = companyId,
                Phone = dbCustomerPhone,
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

            var record = GetCityId_AreaId_ByBranchId(branchId);
            dbCustomerAddress = new Db.CustomerAddressDetail
            {
                Phone = dbCustomerPhone,
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

    internal (int, int) GetCityId_AreaId_ByBranchId(int barnchId)
    {
        int areaId = dbContext.BranchDetails.Where(x => x.BranchId.Equals(barnchId)).Select(x => x.AreaId).FirstOrDefault();
        int cityId = dbContext.Areas.Where(x => x.AreaId.Equals(areaId)).Select(x => x.CityId).FirstOrDefault() ?? 0;
        return (cityId, areaId);
    }

    private async Task<Db.CustomerPhone> SaveCustomerPhone(int companyId, CustomerOrder order)
    {
        Db.CustomerPhone? dbCustomerPhone;
        var cust = order.Customer ?? throw new Exception("Customer is required");
        if (cust.PhoneId == 0)
        {
            dbCustomerPhone = dbContext.CustomerPhones
            .Where(t => t.PhoneNumber != null && cust.Contact != null && t.PhoneNumber.Trim().Equals(cust.Contact.Trim()))
            .FirstOrDefault();
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
}