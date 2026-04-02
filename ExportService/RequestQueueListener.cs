using ExportService.DatabaseContexts;
using ExportService.Entities;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace ExportService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IDbContextFactory<RestaurantsDbContext> pgContextFactory, IDbContextFactory<SqlServerDbContext> sqlContextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.ExportRequestQueue;

        public async override Task OnMessage(string payload)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<ExportServicePayload>(payload);
            if (request == null)
            {
                logger.LogError("Failed to deserialize payload: {Payload}", payload);
                return;
            }
            var connectionString = await GetConnectionString(request.DomainName);
            var orderExists = await CheckIfOrderExists(request.OrderToken, connectionString);

            if (!orderExists)
            {
                var exported = await Export(request.OrderToken, connectionString);
                if (exported)
                {
                    await MarkAsExported(request.OrderToken, connectionString);
                }
            }
            if (request.ExportType == "BranchTransfer")
            {
                await UpdateBranch(request.OrderToken, connectionString);
            }
            if (request.ExportType == "OrderStatusUpdate")
            {
                await UpdateOrderStatus(request.OrderToken, connectionString);
            }
            if (request.ExportType == "RiderAssignment")
            {
                await UpdateRider(request.OrderToken, connectionString);
            }
        }

        private async Task UpdateBranch(string orderToken, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();
            var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderToken == orderToken);
            await sqlContext.OrderMasters
                .Where(x => x.OrderNumber == pgOrderMaster.OrderNumber)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.BranchId, pgOrderMaster.BranchId));
        }

        private async Task UpdateRider(string orderToken, string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            using var postgresContext = GetDbContext(connectionString);
            var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderToken == orderToken);
            var sqlOrderMaster = await sqlContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderNumber == pgOrderMaster.OrderNumber);
            if (pgOrderMaster != null && sqlOrderMaster != null)
            {
                await sqlContext.OrderMasters
                    .Where(x => x.OrderMasterId == sqlOrderMaster.OrderMasterId)
                    .ExecuteUpdateAsync(x => x.SetProperty(x => x.RiderId, pgOrderMaster.RiderId));
            }
        }

        private async Task UpdateOrderStatus(string orderToken, string connectionString)
        {
            using var sqlContext = sqlContextFactory.CreateDbContext();
            using var postgresContext = GetDbContext(connectionString);

            var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderToken == orderToken);
            var orderStatusLogs = await postgresContext.OrderStatusLogs.Where(os => os.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();


            var sqlOrderMaster = await sqlContext.OrderMasters.FirstOrDefaultAsync(om => om.OrderNumber == pgOrderMaster.OrderNumber);

            var sqlOrderStatusLogs = await sqlContext.OrderStatusLogs.Where(os => os.OrderMasterId == sqlOrderMaster.OrderMasterId).ToListAsync();
            var pgOrderStatusLogs = await postgresContext.OrderStatusLogs.Where(os => os.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();
            var createdBy = postgresContext.UserLogins.FirstOrDefault()?.UserId ?? 0;
            foreach (var pgLog in pgOrderStatusLogs)
            {
                if (!sqlOrderStatusLogs.Any(sqlLog => sqlLog.OrderStatusId == pgLog.OrderStatusId))
                {
                    var newSqlLog = new OrderStatusLog
                    {
                        OrderMasterId = sqlOrderMaster.OrderMasterId,
                        OrderStatusId = pgLog.OrderStatusId,
                        CompanyId = pgLog.CompanyId,
                        Description = pgLog.Description,
                        CreatedDate = DateTime.Now,
                        CreatedBy = createdBy,
                    };
                    await sqlContext.OrderStatusLogs.AddAsync(newSqlLog);
                }
            }
            await sqlContext.SaveChangesAsync();
        }

        private async Task<bool> CheckIfOrderExists(string orderToken, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();
            var orderNumber  = await postgresContext.OrderMasters.Where(om => om.OrderToken == orderToken).Select(om => om.OrderNumber).FirstOrDefaultAsync();
            return await sqlContext.OrderMasters.AnyAsync(om => om.OrderNumber == orderNumber);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = pgContextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString.Replace("haproxy", "127.0.0.1") ?? throw new Exception("Restaurant not found");
        }

        private static PostgresDbContext GetDbContext(string connectionString)
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

        private static async Task MarkAsExported(string orderToken, string connectionString)
        {
            using var dbContext = GetDbContext(connectionString);
            await dbContext.OrderMasters.ExecuteUpdateAsync(x => x.SetProperty(x => x.Exported, true));
        }

        private async Task<bool> Export(string orderToken, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();
            var orderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(o => o.OrderToken == orderToken);
            var area = sqlContext.Areas.FirstOrDefault(a => a.AreaId == orderMaster.AreaId);
            var strategy = sqlContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync<bool>(async () =>
            {
                using var transaction = await sqlContext.Database.BeginTransactionAsync();
                try
                {
                    var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(o => o.OrderToken == orderToken);
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
                    if (pgCustomerAddress != null)
                    {
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
                    }

                    var sqlOrderMaster = MapToOrderMaster(pgOrderMaster);
                    sqlOrderMaster.CustomerAddressId = pgCustomerAddress.CustomerAddressId;
                    sqlOrderMaster.CustomerId = pgCustomer.CustomerId;
                    sqlOrderMaster.PhoneId = existingPhone?.PhoneId ?? pgCustomerPhone.PhoneId;
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
                    logger.LogError(ex, "Error exporting order with token: {OrderToken}", orderToken);
                    await transaction.RollbackAsync();
                    return false;
                }
            });
        }

        private static OrderMaster MapToOrderMaster(OrderMaster pgOrderMaster)
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
