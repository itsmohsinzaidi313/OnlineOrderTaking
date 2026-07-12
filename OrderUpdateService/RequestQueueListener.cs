using Microsoft.EntityFrameworkCore;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace OrderUpdateService
{
    public class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, IRabbitMqPublisher publisher, IRestaurantDbContextFactory restaurantDbContextFactory) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderUpdateRequestQueue;

        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderUpdatePayload>(transport);
            object? payload = null;
            using var dbContext = await restaurantDbContextFactory.CreateDbContextByUrlAsync(requestPayload.DomainName);
            try
            {
                var orderMaster = await dbContext.OrderMasters.Where(x => x.OrderToken == requestPayload.OrderToken).FirstOrDefaultAsync();
                string? orderStatusName = null;
                object? orderStatusLogs = null;
                if (orderMaster != null)
                {
                    if (requestPayload.BranchTransferId != null)
                    {
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.BranchId, requestPayload.BranchTransferId.Value));
                        await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
                        {
                            ExportType = "BranchTransfer",
                            OrderNumber = orderMaster.OrderNumber ?? string.Empty,
                        });
                    }

                    if (requestPayload.OrderStatusId != null)
                    {
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.OrderStatusId, requestPayload.OrderStatusId));

                        var previousOrderStatusLog = await dbContext.OrderStatusLogs
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId && x.OrderStatusId == requestPayload.OrderStatusId)
                            .FirstOrDefaultAsync();
                        if (previousOrderStatusLog == null)
                        {
                            dbContext.OrderStatusLogs.Add(new Db.OrderStatusLog
                            {
                                OrderMasterId = orderMaster.OrderMasterId,
                                OrderStatusId = requestPayload.OrderStatusId.Value,
                                CompanyId = orderMaster.CompanyId,
                                Description = string.Empty,
                                CreatedDate = DateTime.UtcNow,
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        orderStatusName = await dbContext.OrderStatuses
                            .Where(x => x.OrderStatusId == requestPayload.OrderStatusId)
                            .Select(x => x.OrderStatusName)
                            .FirstOrDefaultAsync();
                        Func<DateTime, DateTime> convertToPkTime = (dateTime) =>
                        {
                            var karachiTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
                            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), karachiTz);
                        };
                        var statuses = await dbContext.OrderStatuses.ToDictionaryAsync(x => x.OrderStatusId, x => x.OrderStatusName);
                        orderStatusLogs = (await dbContext.OrderStatusLogs
                                                        .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                                                        .ToListAsync())
                                                        .Select(x => new
                                                        {
                                                            Id = x.OrderStatusId,
                                                            Name = statuses[x.OrderStatusId],
                                                            CreatedAt = convertToPkTime(DateTime.SpecifyKind(x.CreatedDate, DateTimeKind.Utc)),
                                                        });
                        await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
                        {
                            ExportType = "OrderStatusUpdate",
                            OrderNumber = orderMaster.OrderNumber ?? string.Empty,
                        });
                    }

                    if (requestPayload.RiderId != null)
                    {
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.RiderId, requestPayload.RiderId));
                        await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
                        {
                            ExportType = "RiderAssignment",
                            OrderNumber = orderMaster.OrderNumber ?? string.Empty,
                        });
                    }

                    int? deliveryTime = requestPayload.DeliveryTime;
                    if (deliveryTime != null)
                    {
                        deliveryTime = orderMaster.DeliveryTime + deliveryTime;
                        await dbContext.OrderMasters
                            .Where(x => x.OrderMasterId == orderMaster.OrderMasterId)
                            .ExecuteUpdateAsync(x => x.SetProperty(x => x.DeliveryTime, deliveryTime));

                        await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
                        {
                            ExportType = "DeliveryTimeUpdate",
                            OrderNumber = orderMaster.OrderNumber ?? string.Empty,
                        });
                    }
                    payload = new
                    {
                        Success = true,
                        Message = "Order updated successfully",
                        OrderStatusName = orderStatusName,
                        OrderStatusLogs = orderStatusLogs,
                        DeliveryTime = deliveryTime,
                    };
                }
                else
                {
                    payload = new
                    {
                        Success = false,
                        Message = "No order found"
                    };
                }

            }
            catch (Exception ex)
            {
                var message = "Error processing order status request: {Message}" + ex.Message;
                logger.LogError(message);
                payload = new
                {
                    Success = false,
                    message,
                };
            }
            var branchUserIds = await dbContext.UserLogins
                .Select(x => x.UserId)
                .ToListAsync();
            var response = new OrderUpdatePayload(requestPayload)
            {
                DataPayload = payload,
                BranchUserIds = branchUserIds,
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderUpdateResponseQueue, response);
        }
    }
}
