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
            await Export(request.OrderToken, connectionString);
            await MarkAsExported(request.OrderToken, connectionString);
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            using var context = pgContextFactory.CreateDbContext();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
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

        private async Task Export(string orderToken, string connectionString)
        {
            using var postgresContext = GetDbContext(connectionString);
            using var sqlContext = sqlContextFactory.CreateDbContext();
            using var transaction = await sqlContext.Database.BeginTransactionAsync();
            try
            {
                var orderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(o => o.OrderToken == orderToken);

                if (orderMaster == null)
                {
                    logger.LogError("Order not found for token: {OrderToken}", orderToken);
                    return;
                }

                var pgOrderMaster = await postgresContext.OrderMasters.FirstOrDefaultAsync(o => o.OrderToken == orderToken);
                var pgOrderDetails = await postgresContext.OrderDetails.Where(od => od.OrderMasterId == pgOrderMaster.OrderMasterId).ToListAsync();
                var pgCustomerPhone = await postgresContext.CustomerPhones.FirstOrDefaultAsync(cp => cp.PhoneId == pgOrderMaster.PhoneId);
                var pgCustomer = await postgresContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == pgOrderMaster.CustomerId);
                var pgCustomerAddress = await postgresContext.CustomerAddressDetails.FirstOrDefaultAsync(ca => ca.CustomerAddressId == pgOrderMaster.CustomerAddressId);

                var sqlOrderMaster = MapToOrderMaster(pgOrderMaster);

                sqlContext.OrderMasters.Add(sqlOrderMaster);
                await sqlContext.SaveChangesAsync();

                await sqlContext.OrderDetails.AddRangeAsync(pgOrderDetails);

                await sqlContext.SaveChangesAsync();

                var phoneExists = await sqlContext.CustomerPhones.AnyAsync(cp => cp.PhoneId == pgCustomerPhone.PhoneId);
                if (!phoneExists)
                {
                    await sqlContext.CustomerPhones.AddAsync(pgCustomerPhone);
                    await sqlContext.SaveChangesAsync();
                }

                var customerExists = await sqlContext.Customers.AnyAsync(c => c.CustomerId == pgCustomer.CustomerId);
                if (!customerExists)
                {
                    await sqlContext.Customers.AddAsync(pgCustomer);
                    await sqlContext.SaveChangesAsync();
                }

                var addressExists = await sqlContext.CustomerAddressDetails.AnyAsync(ca => ca.CustomerAddressId == pgCustomerAddress.CustomerAddressId);
                if (!addressExists)
                {
                    await sqlContext.CustomerAddressDetails.AddAsync(pgCustomerAddress);
                    await sqlContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error exporting order with token: {OrderToken}", orderToken);
                await transaction.RollbackAsync();
            }
        }

        private static OrderMaster MapToOrderMaster(OrderMaster pgOrderMaster)
        {
            return new OrderMaster
            {
                CompanyId = pgOrderMaster.CompanyId,
                OrderNumber = pgOrderMaster.OrderNumber,
                CreatedBy = pgOrderMaster.CreatedBy,
                BranchId = pgOrderMaster.BranchId,
                AreaId = pgOrderMaster.AreaId,
                CustomerId = pgOrderMaster.CustomerId,
                PhoneId = pgOrderMaster.PhoneId,
                CustomerAddressId = pgOrderMaster.CustomerAddressId,
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
