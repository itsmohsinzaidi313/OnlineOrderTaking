using ExportService.DatabaseContexts;
using ExportService.Entities;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;

namespace ExportService
{
    public class OrderExportService(ILogger<OrderExportCycleService> logger, IDbContextFactory<RestaurantsDbContext> pgContextFactory, IDbContextFactory<SqlServerDbContext> sqlContextFactory)
    {
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

            if (exportType == "BranchTransfer")
            {
                await UpdateBranch(orderNumber, connectionString);
            }
            if (exportType == "OrderStatusUpdate")
            {
                await UpdateOrderStatus(orderNumber, connectionString);
            }
            if (exportType == "RiderAssignment")
            {
                await UpdateRider(orderNumber, connectionString);
            }
        }

        private async Task UpdateBranch(string orderNumber, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();

            var orderMaster = await postgresContext.OrderMasters.Where(x => x.OrderNumber == orderNumber).Select(x => new {x.CompanyId, x.BranchId}).FirstOrDefaultAsync();
            await sqlContext.OrderMasters
                .Where(x => x.OrderNumber == orderNumber && x.CompanyId == orderMaster.CompanyId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.BranchId, orderMaster.BranchId));
        }

        private async Task UpdateRider(string orderNumber, string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            using var postgresContext = GetDbContext(connectionString);

            var orderMaster = await postgresContext.OrderMasters.Where(x => x.OrderNumber == orderNumber).Select(x => new { x.CompanyId, x.RiderId }).FirstOrDefaultAsync();

            await sqlContext.OrderMasters
                .Where(x => x.OrderNumber == orderNumber && x.CompanyId == orderMaster.CompanyId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.RiderId, orderMaster.RiderId));
        }

        private async Task UpdateOrderStatus(string orderNumber, string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            using var postgresContext = GetDbContext(connectionString);
            var companyId = await GetCompanyIdAsync(connectionString);
            var sqlOrderStatusLogs = await sqlContext.OrderStatusLogs
                .Join(sqlContext.OrderMasters, a => a.OrderMasterId, b => b.OrderMasterId, (a, b) => new { a, b })
                .Where(os => os.b.OrderNumber == orderNumber && os.b.CompanyId == companyId).ToListAsync();

            var pgOrderStatusLogs = await postgresContext.OrderStatusLogs
                .Join(postgresContext.OrderMasters, a => a.OrderMasterId, b => b.OrderMasterId, (a, b) => new { a, b })
                .Where(os => os.b.OrderNumber == orderNumber).ToListAsync();

            var createdBy = postgresContext.UserLogins.FirstOrDefault()?.UserId ?? 0;

            var sqlOrderMasterId = sqlOrderStatusLogs.First().b.OrderMasterId;

            foreach (var pgLog in pgOrderStatusLogs)
            {
                if (!sqlOrderStatusLogs.Any(sqlLog => sqlLog.a.OrderStatusId == pgLog.a.OrderStatusId))
                {
                    var newSqlLog = new OrderStatusLog
                    {
                        OrderMasterId = sqlOrderMasterId,
                        OrderStatusId = pgLog.a.OrderStatusId,
                        CompanyId = pgLog.a.CompanyId,
                        Description = pgLog.a.Description,
                        CreatedDate = pgLog.a.CreatedDate,
                        CreatedBy = createdBy,
                    };
                    await sqlContext.OrderStatusLogs.AddAsync(newSqlLog);
                }
            }
            await sqlContext.SaveChangesAsync();
            var latestStatusId = pgOrderStatusLogs.OrderByDescending(x => x.a.OrderStatusLogId).First().a.OrderStatusId;
            await sqlContext.OrderMasters.Where(x => x.OrderMasterId == sqlOrderMasterId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.OrderStatusId, latestStatusId));
        }
        private async Task<int> GetCompanyIdAsync(string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            return await sqlContext.SetupCompanies.Select(c => c.CompanyId).FirstAsync();
        }

        private async Task<bool> CheckIfOrderExists(string orderNumber, string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            var companyId = await GetCompanyIdAsync(connectionString);
            return await sqlContext.OrderMasters.AnyAsync(om => om.OrderNumber == orderNumber && om.CompanyId == companyId);
        }

        internal async Task<string> GetConnectionString(string domainName)
        {
            using var context = pgContextFactory.CreateDbContext();
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
                    var pgCustomerPhone = await postgresContext.CustomerPhones.FirstOrDefaultAsync(cp => cp.PhoneId == pgOrderMaster.PhoneId);
                    var pgCustomer = await postgresContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == pgOrderMaster.CustomerId);
                    var pgCustomerAddress = await postgresContext.CustomerAddressDetails.FirstOrDefaultAsync(ca => ca.CustomerAddressId == pgOrderMaster.CustomerAddressId);

                    var createdBy = postgresContext.UserLogins.FirstOrDefault()?.UserId ?? 0;
                    var createdDate = DateTime.Now;

                    var existingPhone = await sqlContext.CustomerPhones.FirstOrDefaultAsync(cp => cp.PhoneNumber == pgCustomerPhone.PhoneNumber);
                    if (existingPhone == null)
                    {
                        pgCustomerPhone.PhoneId = 0;
                        pgCustomerPhone.CreatedBy = createdBy;
                        pgCustomerPhone.CreatedDate = createdDate;
                        await sqlContext.CustomerPhones.AddAsync(pgCustomerPhone);
                        await sqlContext.SaveChangesAsync();
                    }

                    var existingCustomer = await sqlContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == pgCustomer.CustomerId);
                    if (existingCustomer == null)
                    {
                        pgCustomer.CustomerId = 0;
                        pgCustomer.PhoneId = existingPhone?.PhoneId ?? pgCustomerPhone.PhoneId;
                        await sqlContext.Customers.AddAsync(pgCustomer);
                        await sqlContext.SaveChangesAsync();
                    }
                    var existingAddress = await sqlContext.CustomerAddressDetails.FirstOrDefaultAsync(ca => ca.CompleteAddress == pgCustomerAddress.CompleteAddress);
                    if (existingAddress == null)
                    {
                        pgCustomerAddress.CustomerAddressId = 0;
                        pgCustomerAddress.PhoneId = existingPhone?.PhoneId ?? pgCustomerPhone.PhoneId;
                        pgCustomerAddress.CreatedBy = createdBy;
                        pgCustomerAddress.CreatedDate = createdDate;
                        pgCustomerAddress.Area = area;
                        await sqlContext.CustomerAddressDetails.AddAsync(pgCustomerAddress);
                        await sqlContext.SaveChangesAsync();
                    }

                    var sqlOrderMaster = MapToOrderMaster(pgOrderMaster);
                    sqlOrderMaster.CustomerAddressId = existingAddress?.CustomerAddressId ?? pgCustomerAddress.CustomerAddressId;
                    sqlOrderMaster.CustomerId = existingCustomer?.CustomerId ?? pgCustomer.CustomerId;
                    sqlOrderMaster.PhoneId = existingPhone?.PhoneId ?? pgCustomerPhone.PhoneId;
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

                    var pgOrderStatuses = await postgresContext.OrderStatusLogs.Where(os => os.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();
                    pgOrderStatuses.ForEach(x =>
                    {
                        x.OrderStatusLogId = 0;
                        x.OrderMasterId = sqlOrderMaster.OrderMasterId;
                        x.CreatedDate = createdDate;
                        x.CreatedBy = createdBy;
                    });

                    await sqlContext.OrderStatusLogs.AddRangeAsync(pgOrderStatuses);
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
            return new OrderMaster
            {
                CompanyId = pgOrderMaster.CompanyId,
                OrderNumber = pgOrderMaster.OrderNumber,
                CreatedBy = pgOrderMaster.CreatedBy,
                CreatedDate = DateTime.Now,
                BranchId = pgOrderMaster.BranchId,
                AreaId = pgOrderMaster.AreaId,
                RiderId = pgOrderMaster.RiderId,
                OrderStatusId = pgOrderMaster.OrderStatusId,
                IsAdvanceOrder = pgOrderMaster.IsAdvanceOrder,
                SpecialInstruction = pgOrderMaster.SpecialInstruction,
                OrderDate = pgOrderMaster.OrderDate,
                OrderTime = pgOrderMaster.OrderTime,
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
