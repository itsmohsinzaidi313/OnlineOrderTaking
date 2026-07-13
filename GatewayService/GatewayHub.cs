using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GatewayService
{

    [Authorize]
    public class GatewayHub(ILogger<GatewayHub> logger, Implementation implementation, IConnectionMultiplexer redis) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetClientOnlineAsync(userId, Context.ConnectionId);
            await implementation.SendPendingPayload(userId);
            ConfigureTenantHeaders();
            await base.OnConnectedAsync();
        }

        private void ConfigureTenantHeaders()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var tenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                var originalHost = httpContext.Request.Headers["X-Original-Host"].FirstOrDefault();

                Context.Items["TenantId"] = tenantId;
                Context.Items["OriginalHost"] = originalHost;
                logger.LogInformation("Tenant headers configured: TenantId={TenantId}, OriginalHost={OriginalHost}", tenantId, originalHost);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetUserOfflineAsync(userId);
            await base.OnDisconnectedAsync(ex);
        }

        private void LogTenant()
        {
            var tenantId = Context.Items["TenantId"]?.ToString();
            var originalHost = Context.Items["OriginalHost"]?.ToString();
            logger.LogInformation("Tenant Info: TenantId={TenantId}, OriginalHost={OriginalHost}", tenantId, originalHost);
        }

        public async Task MenuRequest(string domainName, int branchId, string responseKey)
        {
            LogTenant();
            var db = redis.GetDatabase();
            var response = await db.StringGetAsync($"{domainName}:{branchId}:menu");
            if (!response.IsNull)
            {
                var payload = JsonSerializer.Deserialize<DataServicePayload>(response.ToString());
                await Clients.Caller.SendAsync("Ack", new { status = "cached" });
                await Clients.Caller.SendAsync(responseKey, payload);
                return;
            }
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = string.Empty,
                BranchId = branchId,
                ResponseKey = responseKey,
                SignalRMethodName = "MenuRequest"
            }.FillContext(Context);

            await QueuePayload(RabbitMqQueues.MenuRequestQueue, obj);
        }

        public async Task DeliveryAndPickupRequest(string domainName, int branchId, string responseKey)
        {
            LogTenant();
            var db = redis.GetDatabase();
            var response = await db.StringGetAsync($"{domainName}:{branchId}:dandp");
            if (!response.IsNull)
            {
                response = DataHandler(response);
                var payload = JsonSerializer.Deserialize<DataServicePayload>(response.ToString());
                await Clients.Caller.SendAsync("Ack", new { status = "cached" });
                await Clients.Caller.SendAsync(responseKey, payload);
                return;
            }
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = string.Empty,
                BranchId = branchId,
                ResponseKey = responseKey,
                SignalRMethodName = "DeliveryAndPickupRequest"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.SettingRequestQueue, obj);
        }

        public async Task CustomerOrderHistory(string domainName, string orderToken, string responseKey)
        {
            LogTenant();
            var obj = new CustomerOrderHistoryServicePayload
            {
                DomainName = domainName,
                ResponseKey = responseKey,
                SignalRMethodName = "CustomerOrderHistory",
                OrderToken = orderToken
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.CustomerOrderHistoryRequestQueue, obj);
        }

        public async Task OrderHistoryRequest(string domainName, int userId, string responseKey)
        {
            LogTenant();
            var obj = new DataServicePayload
            {
                DomainName = domainName,
                DataRequestType = "Orders",
                ResponseKey = responseKey,
                SignalRMethodName = "OrderHistoryRequest"
            }.FillContext(Context);
            obj.OrderUserId = userId;
            await QueuePayload(RabbitMqQueues.OrderHistoryRequestQueue, obj);
        }

        public async Task PlaceOrder(CustomerOrder order, string responseKey)
        {
            LogTenant();
            if (order.Items.Count == 0)
            {
                await Clients.Caller.SendAsync(responseKey, new { Success = false, Message = "Order must contain at least one item." });
                return;
            }
            var obj = new OrderServicePayload
            {
                Order = order,
                BranchId = order.BranchId,
                DomainName = order.Domain,
                ResponseKey = responseKey,
                SignalRMethodName = "PlaceOrder"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.OrderRequestQueue, obj);
        }

        public async Task OrderStatus(string domainName, int branchId, string orderNumber, int? orderStatusId, int? branchTransferId, int? riderId, int? deliveryTime, string responseKey)
        {
            LogTenant();
            var obj = new OrderUpdatePayload
            {
                DomainName = domainName,
                BranchId = branchId,
                OrderToken = orderNumber,
                ResponseKey = responseKey,
                BranchTransferId = branchTransferId,
                OrderStatusId = orderStatusId,
                DeliveryTime = deliveryTime,
                RiderId = riderId,
                SignalRMethodName = "OrderStatus"
            }.FillContext(Context);
            await QueuePayload(RabbitMqQueues.OrderUpdateRequestQueue, obj);
        }

        private async Task QueuePayload<T>(string queues, T payload)
        {
            await implementation.QueueRequestPayload(queues, payload);
            await Clients.Caller.SendAsync("Ack", new { status = "queued" });
        }

        internal string ExtractUserIdFromClaims()
        {
            return Context.User?.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        }
        private static string DataHandler(string svcPayload)
        {
            var rootNode = JsonNode.Parse(svcPayload) as JsonObject;
            if (rootNode is null)
            {
                return svcPayload;
            }

            if (rootNode["DataPayload"] is not JsonObject dataPayload)
            {
                return svcPayload;
            }

            UpdateBranchStatus(dataPayload["Pickup"]);
            UpdateBranchStatus(dataPayload["Delivery"]);

            return rootNode.ToJsonString();
        }

        private static void UpdateBranchStatus(JsonNode? serviceTypeNode)
        {
            if (serviceTypeNode is not JsonObject serviceType)
            {
                return;
            }

            foreach (var city in serviceType)
            {
                if (city.Value is not JsonObject cityObject || cityObject["Branches"] is not JsonArray branches)
                {
                    continue;
                }

                foreach (var branchNode in branches)
                {
                    if (branchNode is not JsonObject branchObject)
                    {
                        continue;
                    }

                    branchObject["IsBranchOpen"] = CalculateBranchOpenStatus(branchObject["BusinessDays"] as JsonArray);
                }
            }
        }

        private static bool CalculateBranchOpenStatus(JsonArray? businessTimes)
        {
            if (businessTimes is null || businessTimes.Count == 0)
            {
                return false;
            }

            var karachiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, karachiTimeZone);

            var currentDay = now.DayOfWeek;
            var businessTimeNode = businessTimes.FirstOrDefault(x => x["Day"]?.GetValue<string>()?.Equals(currentDay.ToString(), StringComparison.OrdinalIgnoreCase) == true);

            if (businessTimeNode is not JsonObject businessTime)
                return false;

            var day = businessTime["Day"]?.GetValue<string>();
            if (!Enum.TryParse<DayOfWeek>(day, true, out var businessDay))
                return false;

            if (!TryParseTime(businessTime["StartTime"], out var startTime) || !TryParseTime(businessTime["EndTime"], out var endTime))
                return false;

            var startDateTime = now.Date.Add(startTime);
            var endDateTime = now.Date.Add(endTime);
            if (endTime < TimeSpan.FromHours(12))
            {
                endDateTime = endDateTime.AddDays(1);
            }
            if (startDateTime < now && now < endDateTime)
            {
                return true;
            }
            else if (startTime == endTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool TryParseTime(JsonNode? timeNode, out TimeSpan time)
        {
            time = default;
            if (timeNode is null)
            {
                return false;
            }

            var timeValue = timeNode.GetValue<string>();
            return TimeSpan.TryParse(timeValue, CultureInfo.InvariantCulture, out time);
        }
    }
}
