using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GatewayService.ServiceResponseListeners
{
    public class SettingsDataServiceResponseListener(ILogger<SettingsDataServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation, IConnectionMultiplexer redis) : RabbitMqConsumerService<SettingsDataServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.SettingResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            svcPayload = DataHandler(svcPayload);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var branchId = root.GetProperty("BranchId").GetInt32();
            var domainName = root.GetProperty("DomainName").GetString() ?? throw new Exception("DomainName not found");
            var success = root.GetProperty("Success").GetBoolean();
            if (success)
            {
                await redis.GetDatabase().StringSetAsync($"{domainName}:{branchId}:dandp", svcPayload);
            }

            await implementation.SendToUser<DataServicePayload>(svcPayload);
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
