using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using Db = PointofSaleModels.PGDatabaseModels;

namespace CreateOrderService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection, Implementation impl, IRabbitMqPublisher publisher) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderRequestQueue;
        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<OrderServicePayload>(transport);
            object? response = null;
            try
            {
                if (requestPayload == null)
                {
                    logger.LogWarning("Invalid or missing order payload for company {CompanyId}, branch {BranchId}", requestPayload?.RestaurantId, requestPayload?.BranchId);
                    throw new InvalidOperationException("Invalid order payload");
                }
                var url = requestPayload.DomainName;
                await impl.SaveOrderAsync(url, requestPayload.Order!);
                var orderToken = requestPayload.Order.OrderToken ?? throw new Exception("Order token not generated");
                await impl.SaveToken(url, orderToken);
                requestPayload.Order.OrderStatusLogs = await impl.OrderStatusLogs(url, orderToken);
                response = new { Success = true, Message = "Order processed successfully", OrderNumber = orderToken };
                await foreach (var userId in impl.GetBranchUsersIdsAsync(url, requestPayload.BranchId))
                {
                    var order = requestPayload.Order;
                    await publisher.PublishToQueueAsync(RabbitMqQueues.PushNotificationRequestQueue, new PushNotificationServicePayload
                    {
                        ClientId = $"branch:{userId}:*",
                        Title = "New Order Received!",
                        Message = $" New order received from the {order?.BranchName} branch - Order# {order?.OrderToken} — Rs.{double.Round(order?.AmountWithGst ?? 0.0 + order.DeliveryCharges ?? 0)}.",
                    });

                }
                await publisher.PublishToQueueAsync(RabbitMqQueues.OrderHistoryRequestQueue,
                   new DataServicePayload(requestPayload)
                   {
                       OrderToken = requestPayload.Order.OrderToken
                   });
                await publisher.PublishToQueueAsync(RabbitMqQueues.ExportRequestQueue, new ExportServicePayload(requestPayload)
                {
                    ExportType = "NewOrder",
                    OrderNumber = requestPayload.Order.OrderNumber ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
            response = new OrderServicePayload(requestPayload)
            {
                DataPayload = response
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.OrderResponseQueue, response);
        }

        
    }
}
