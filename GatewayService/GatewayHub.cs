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
    public class GatewayHub(Implementation implementation, IConnectionMultiplexer redis) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetClientOnlineAsync(userId, Context.ConnectionId);
            await implementation.SendPendingPayload(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string userId = ExtractUserIdFromClaims();
            await implementation.SetUserOfflineAsync(userId);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task MenuRequest(string domainName, int branchId, string responseKey)
        {
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

            var now = DateTime.Now;
            var currentDay = now.DayOfWeek.ToString();
            var currentTime = now.TimeOfDay;

            foreach (var businessTimeNode in businessTimes)
            {
                if (businessTimeNode is not JsonObject businessTime)
                {
                    continue;
                }

                var day = businessTime["Day"]?.GetValue<string>();
                if (!string.Equals(day, currentDay, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!TryParseTime(businessTime["StartTime"], out var startTime) || !TryParseTime(businessTime["EndTime"], out var endTime))
                {
                    continue;
                }

                if (startTime <= endTime)
                {
                    if (currentTime >= startTime && currentTime <= endTime)
                    {
                        return true;
                    }
                }
                else
                {
                    if (currentTime >= startTime || currentTime <= endTime)
                    {
                        return true;
                    }
                }
            }

            return false;
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
