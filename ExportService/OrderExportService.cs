using ExportService.DatabaseContexts;
using ExportService.Entities;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;

namespace ExportService
{
    public class OrderExportService(ILogger<OrderExportCycleService> logger, IDbContextFactory<RestaurantsDbContext> pgContextFactory, IDbContextFactory<SqlServerDbContext> sqlContextFactory)
    {
        readonly TimeZoneInfo karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
        DateTime ConvertToPkTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), karachiTz);
        }

        public async Task OnMessageHandler(ExportServicePayload request, string connectionString)
        {
            var orderNumber = request.OrderNumber;
            var exportType = request.ExportType;


            var orderExists = await CheckIfOrderExists(orderNumber, connectionString);
            if (!orderExists)
            {
                var exported = await Export(orderNumber, connectionString);
                if (exported)
                {
                    await MarkAsExported(orderNumber, connectionString);
                }
            }
            else
            {
                await MarkAsExported(orderNumber, connectionString);
            }

            if (exportType == "OrderStatusUpdate")
            {
                await UpdateOrderStatus(orderNumber, connectionString);
            }
            if (new[] { "DeliveryTimeUpdate", "RiderAssignment", "BranchTransfer" }.Contains(exportType))
            {
                await UpdateOrder(orderNumber, connectionString);
            }
        }

        private async Task UpdateOrder(string orderNumber, string connectionString)
        {
            await using var postgresContext = GetDbContext(connectionString);
            await using var sqlContext = sqlContextFactory.CreateDbContext();

            var orderMaster = await postgresContext.OrderMasters.Where(x => x.OrderNumber == orderNumber).FirstOrDefaultAsync();

            await sqlContext.OrderMasters
                .Where(x => x.OrderNumber == orderNumber && x.CompanyId == orderMaster.CompanyId)
                .ExecuteUpdateAsync(x =>
                x.SetProperty(x => x.BranchId, orderMaster.BranchId)
                .SetProperty(x => x.DeliveryTime, orderMaster.DeliveryTime)
                .SetProperty(x => x.RiderId, orderMaster.RiderId));
        }

        private async Task UpdateOrderStatus(string orderNumber, string connectionString)
        {
            await using var sqlContext = sqlContextFactory.CreateDbContext();
            await using var postgresContext = GetDbContext(connectionString);
            var companyId = await GetCompanyIdAsync(connectionString);
            var pgOrderMaster = await postgresContext.OrderMasters
                                            .Where(x => x.OrderNumber == orderNumber)
                                            .FirstAsync();
            var sqlOrderMaster = await sqlContext.OrderMasters
                                            .Where(x => x.OrderNumber == orderNumber && x.CompanyId == companyId)
                                            .FirstAsync();
            var pgOrderMasterId = pgOrderMaster.OrderMasterId;
            var sqlOrderMasterId = sqlOrderMaster.OrderMasterId;
            var pgOrderStatusLogs = await postgresContext.OrderStatusLogs
                .Where(x => x.OrderMasterId == pgOrderMasterId)
                .ToListAsync();

            var sqlOrderStatusLogs = await sqlContext.OrderStatusLogs
                .Where(os => os.OrderMasterId == sqlOrderMasterId)
                .ToListAsync();

            var createdBy = postgresContext.UserLogins.FirstOrDefault()?.UserId ?? 0;
            var confirmedId = postgresContext.OrderStatuses.Where(x => x.OrderStatusName == "Confirmed").Select(x => x.OrderStatusId).FirstOrDefault();

            foreach (var pgLog in pgOrderStatusLogs)
            {
                if (!sqlOrderStatusLogs.Any(sqlLog => sqlLog.OrderStatusId == pgLog.OrderStatusId))
                {
                    var newSqlLog = new OrderStatusLog
                    {
                        OrderMasterId = sqlOrderMasterId,
                        OrderStatusId = pgLog.OrderStatusId,
                        CompanyId = pgLog.CompanyId,
                        Description = pgLog.Description,
                        CreatedDate = pgLog.CreatedDate,
                        CreatedBy = createdBy,
                        IsActive = true,
                    };
                    await sqlContext.OrderStatusLogs.AddAsync(newSqlLog);
                    if (newSqlLog.OrderStatusId == confirmedId)
                    {
                        var orderTime = pgOrderMaster.OrderTime;
                        var orderDate = pgOrderMaster.OrderDate ?? throw new Exception("Invalid order date");

                        var pgDateTime = new DateTime(DateOnly.FromDateTime(orderDate), TimeOnly.FromTimeSpan(orderTime ?? TimeSpan.Zero));
                        DateTime orderDateTime = ConvertToPkTime(pgDateTime);

                        orderDate = new DateTime(DateOnly.FromDateTime(orderDateTime), TimeOnly.MinValue);
                        orderTime = orderDateTime.TimeOfDay;
                        var orderMasterLog = new OrderMasterLog
                        {
                            OrderMasterId = sqlOrderMaster.OrderMasterId,
                            CompanyId = sqlOrderMaster.CompanyId,
                            BranchId = sqlOrderMaster.BranchId,
                            OrderStatusId = newSqlLog.OrderStatusId,
                            OrderDate = orderDate,
                            OrderTime = orderTime,
                            CreatedDate = pgLog.CreatedDate,
                            IsActive = true,
                            IsSyncToPos = false
                        };
                        await sqlContext.OrderMasterLogs.AddAsync(orderMasterLog);
                    }
                }
            }
            await sqlContext.SaveChangesAsync();
            var latestStatusId = pgOrderStatusLogs.OrderByDescending(x => x.OrderStatusLogId).First().OrderStatusId;
            await sqlContext.OrderMasters.Where(x => x.OrderMasterId == sqlOrderMasterId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.OrderStatusId, latestStatusId));
        }
        private async Task<int> GetCompanyIdAsync(string connectionString)
        {
            await using var postgresContext = GetDbContext(connectionString);
            return await postgresContext.SetupCompanies.Select(x => x.CompanyId).FirstAsync();
        }

        private async Task<bool> CheckIfOrderExists(string orderNumber, string connectionString)
        {
            await using var sqlContext = sqlContextFactory.CreateDbContext();
            var companyId = await GetCompanyIdAsync(connectionString);
            var ordermaster = await sqlContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderNumber == orderNumber && om.CompanyId == companyId);
            return ordermaster != null;
        }

        internal async Task<string> GetConnectionString(string domainName)
        {
            await using var context = pgContextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }

        public PostgresDbContext GetDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .Options;
            return new PostgresDbContext(options);
        }

        private async Task MarkAsExported(string orderNumber, string connectionString)
        {
            using var dbContext = GetDbContext(connectionString);
            await dbContext.OrderMasters
                .Where(om => om.OrderNumber == orderNumber)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Exported, true));
        }

        private async Task<bool> Export(string orderNumber, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();
            var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            var area = sqlContext.Areas.FirstOrDefault(a => a.AreaId == pgOrderMaster.AreaId);
            var companyId = pgOrderMaster.CompanyId;
            var strategy = sqlContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync<bool>(async () =>
            {
                using var transaction = await sqlContext.Database.BeginTransactionAsync();
                try
                {
                    var pgOrderDetails = await postgresContext.OrderDetails.Where(od => od.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();
                    var pgPhone = await postgresContext.CustomerPhones.FirstOrDefaultAsync(cp => cp.PhoneId == pgOrderMaster.PhoneId);
                    var pgCustomer = await postgresContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == pgOrderMaster.CustomerId);
                    var pgAddress = await postgresContext.CustomerAddressDetails.FirstOrDefaultAsync(ca => ca.CustomerAddressId == pgOrderMaster.CustomerAddressId);

                    var createdBy = postgresContext.UserLogins.FirstOrDefault()?.UserId ?? 0;
                    var createdDate = ConvertToPkTime(DateTime.Now);

                    var sqlOrderMaster = MapToOrderMaster(pgOrderMaster);
                    var existingPhone = await sqlContext.CustomerPhones.FirstOrDefaultAsync(cp => cp.PhoneNumber == pgPhone.PhoneNumber && cp.CompanyId == companyId);
                    if (existingPhone == null)
                    {
                        pgPhone.PhoneId = 0;
                        pgPhone.CreatedBy = createdBy;
                        pgPhone.CreatedDate = createdDate;
                        pgPhone.IsActive = true;
                        await sqlContext.CustomerPhones.AddAsync(pgPhone);
                        await sqlContext.SaveChangesAsync();
                        sqlOrderMaster.PhoneId = pgPhone.PhoneId;

                        pgCustomer.CustomerId = 0;
                        pgCustomer.PhoneId = pgPhone.PhoneId;

                        await sqlContext.Customers.AddAsync(pgCustomer);
                        await sqlContext.SaveChangesAsync();
                        sqlOrderMaster.CustomerId = pgCustomer.CustomerId;
                        if (pgAddress != null)
                        {
                            pgAddress.CustomerAddressId = 0;
                            pgAddress.PhoneId = pgPhone.PhoneId;
                            pgAddress.CreatedBy = createdBy;
                            pgAddress.CreatedDate = createdDate;
                            pgAddress.Area = area;
                            pgAddress.IsActive = true;
                            await sqlContext.CustomerAddressDetails.AddAsync(pgAddress);
                            await sqlContext.SaveChangesAsync();
                            sqlOrderMaster.CustomerAddressId = pgAddress.CustomerAddressId;
                        }
                    }
                    else
                    {
                        var existingCustomer = await sqlContext.Customers.FirstOrDefaultAsync(c => c.PhoneId == existingPhone.PhoneId && c.CustomerName == pgCustomer.CustomerName && c.CompanyId == companyId);
                        if (existingCustomer == null)
                        {
                            pgCustomer.CustomerId = 0;
                            pgCustomer.PhoneId = existingPhone.PhoneId;
                            pgCustomer.IsActive = true;
                            await sqlContext.Customers.AddAsync(pgCustomer);
                            await sqlContext.SaveChangesAsync();
                        }
                        sqlOrderMaster.CustomerId = existingCustomer?.CustomerId ?? pgCustomer.CustomerId;
                        if (pgAddress != null)
                        {
                            var existingAddress = await sqlContext.CustomerAddressDetails.FirstOrDefaultAsync(ca => ca.CompleteAddress == pgAddress.CompleteAddress && ca.PhoneId == existingPhone.PhoneId);
                            if (existingAddress == null)
                            {
                                pgAddress.CustomerAddressId = 0;
                                pgAddress.PhoneId = existingPhone.PhoneId;
                                pgAddress.CreatedBy = createdBy;
                                pgAddress.CreatedDate = createdDate;
                                pgAddress.Area = area;
                                pgAddress.IsActive = true;
                                await sqlContext.CustomerAddressDetails.AddAsync(pgAddress);
                                await sqlContext.SaveChangesAsync();
                            }
                            sqlOrderMaster.CustomerAddressId = existingAddress?.CustomerAddressId ?? pgAddress.CustomerAddressId;
                        }
                    }
                    var phoneId = existingPhone?.PhoneId ?? pgPhone.PhoneId;

                    sqlOrderMaster.PhoneId = phoneId;
                    if (pgOrderMaster.RiderId != null)
                    {
                        var rider = await sqlContext.Riders.FirstOrDefaultAsync(x => x.RiderId == pgOrderMaster.RiderId);
                        sqlOrderMaster.Rider = rider;
                    }

                    sqlContext.OrderMasters.Add(sqlOrderMaster);
                    await sqlContext.SaveChangesAsync();
                    pgOrderDetails.ForEach(x =>
                    {
                        x.OrderMaster = sqlOrderMaster;
                        x.OrderDetailId = 0;
                        x.CreatedBy = createdBy;
                        x.CreatedDate = createdDate;
                    });
                    await sqlContext.OrderDetails.AddRangeAsync(pgOrderDetails);

                    await sqlContext.SaveChangesAsync();

                    var pgOrderStatusLogs = await postgresContext.OrderStatusLogs.Where(os => os.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();
                    pgOrderStatusLogs.ForEach(x =>
                    {
                        x.OrderStatusLogId = 0;
                        x.OrderMasterId = sqlOrderMaster.OrderMasterId;
                        x.CreatedDate = createdDate;
                        x.CreatedBy = createdBy;
                        x.IsActive = true;
                    });

                    await sqlContext.OrderStatusLogs.AddRangeAsync(pgOrderStatusLogs);
                    await sqlContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error exporting order with token: {OrderToken}", pgOrderMaster.OrderToken);
                    await transaction.RollbackAsync();
                    return false;
                }
            });
        }

        private OrderMaster MapToOrderMaster(OrderMaster pgOrderMaster)
        {
            var orderTime = pgOrderMaster.OrderTime;
            var orderDate = pgOrderMaster.OrderDate ?? throw new Exception("Invalid order date");

            var pgDateTime = new DateTime(DateOnly.FromDateTime(orderDate), TimeOnly.FromTimeSpan(orderTime ?? TimeSpan.Zero));
            DateTime orderDateTime = ConvertToPkTime(pgDateTime);

            orderDate = new DateTime(DateOnly.FromDateTime(orderDateTime), TimeOnly.MinValue);
            orderTime = orderDateTime.TimeOfDay;

            return new OrderMaster
            {
                CompanyId = pgOrderMaster.CompanyId,
                OrderNumber = pgOrderMaster.OrderNumber,
                CreatedBy = pgOrderMaster.CreatedBy,
                CreatedDate = ConvertToPkTime(DateTime.Now),
                BranchId = pgOrderMaster.BranchId,
                AreaId = pgOrderMaster.AreaId,
                RiderId = pgOrderMaster.RiderId,
                OrderStatusId = pgOrderMaster.OrderStatusId,
                IsAdvanceOrder = pgOrderMaster.IsAdvanceOrder,
                SpecialInstruction = pgOrderMaster.SpecialInstruction,
                OrderDate = orderDate,
                OrderTime = orderTime,
                TotalAmountWithoutGst = pgOrderMaster.TotalAmountWithoutGst,
                Gstid = pgOrderMaster.Gstid,
                TotalAmountWithGst = pgOrderMaster.TotalAmountWithGst,
                IsActive = pgOrderMaster.IsActive,
                AlternateNumber = pgOrderMaster.AlternateNumber,
                AdvanceOrderDate = pgOrderMaster.AdvanceOrderDate,
                DeliveryTime = pgOrderMaster.DeliveryTime,
                Clinumber = pgOrderMaster.Clinumber,
                OrderSourceId = pgOrderMaster.OrderSourceId,
                OrderSourceValue = pgOrderMaster.OrderSourceValue,
                DiscountId = pgOrderMaster.DiscountId,
                DeliveryCharges = pgOrderMaster.DeliveryCharges,
                OrderCancelReasonId = pgOrderMaster.OrderCancelReasonId,
                WaiterId = pgOrderMaster.WaiterId,
                ShiftDetailId = pgOrderMaster.ShiftDetailId,
                TerminalDetailId = pgOrderMaster.TerminalDetailId,
                OrderModeId = pgOrderMaster.OrderModeId,
                Cover = pgOrderMaster.Cover,
                PaymentTypeId = pgOrderMaster.PaymentTypeId,
                DiscountAmount = pgOrderMaster.DiscountAmount,
                Gstamount = pgOrderMaster.Gstamount,
                CareOfId = pgOrderMaster.CareOfId,
                BillPrintCount = pgOrderMaster.BillPrintCount,
                PreviousOrderMasterId = pgOrderMaster.PreviousOrderMasterId,
                Remarks = pgOrderMaster.Remarks,
                DiscountPercent = pgOrderMaster.DiscountPercent,
                Gstpercent = pgOrderMaster.Gstpercent,
                FinishWasteRemarks = pgOrderMaster.FinishWasteRemarks,
                FinishWasteReasonId = pgOrderMaster.FinishWasteReasonId,
                TableId = pgOrderMaster.TableId,
                EmailAddress = pgOrderMaster.EmailAddress,
                OrderJson = pgOrderMaster.OrderJson,
                SrbInvoiceId = pgOrderMaster.SrbInvoiceId,
                FbrInvoiceId = pgOrderMaster.FbrInvoiceId,
                ReservationId = pgOrderMaster.ReservationId,
                TotalAdvance = pgOrderMaster.TotalAdvance,
                IsSyncToPos = pgOrderMaster.IsSyncToPos,
                Tip = pgOrderMaster.Tip,
                ChangeAmount = pgOrderMaster.ChangeAmount,
                VoucherCode = pgOrderMaster.VoucherCode,
                VoucherId = pgOrderMaster.VoucherId,
                VoucherAmount = pgOrderMaster.VoucherAmount,
                CareOfName = pgOrderMaster.CareOfName,
                BankName = pgOrderMaster.BankName,
                CardNumber = pgOrderMaster.CardNumber,
                PartyPhoneId = pgOrderMaster.PartyPhoneId,
                PartyCustomerId = pgOrderMaster.PartyCustomerId,
                OrderDetails = []
            };
        }
    }
}
