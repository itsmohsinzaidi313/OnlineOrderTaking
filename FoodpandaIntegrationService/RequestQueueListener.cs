using PointofSaleModels.Integrations;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System.Net.Http.Headers;

namespace FoodpandaIntegrationService
{
    internal class RequestQueueListener(ILogger<RequestQueueListener> logger, RabbitMqConnection rabbitConnection) : RabbitMqConsumerService<RequestQueueListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.FoodpandaIntegrationRequestQueue;
        public override async Task OnMessage(string transport)
        {
            var requestPayload = System.Text.Json.JsonSerializer.Deserialize<IntegrationServicePayload<FoodPandaPayloadModel>>(transport);
            object? response = null;
            try
            {
                var order = requestPayload?.OrderPayload ?? throw new Exception("Order payload is missing");
                var url = order?.CallbackUrls?.OrderAcceptedUrl ?? throw new Exception("Order accepted URL is missing");
                var orderToken = order.Token ?? throw new Exception("Order token is missing");
                var orderCode = order.Code ?? throw new Exception("Order code is missing");
                var accessToken = await RequestAccessTokenAsync() ?? throw new Exception("Access token is missing");

                await OrderAcceptedStatus(accessToken, orderToken, orderCode, url.ToString());
                var commonPayload = order.Payload;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing order request message");
                response = new { Success = false, Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message };
            }
        }

        private static async Task OrderAcceptedStatus(string accessToken, string orderToken, string orderCode, string url)
        {
            var content = JsonContent.Create(new
            {
                acceptanceTime = DateTime.Now,
                remoteOrderId = orderCode,
                status = "order_accepted"
            });
            using var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = content,
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST {client.BaseAddress}{request.RequestUri} -> {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        private static async Task<string?> RequestAccessTokenAsync()
        {
            const string baseUrl = "https://integration-middleware.as.restaurant-partners.com";
            const string loginPath = "/v2/login";
            const string username = "as-plugin-y-generation-systems-005";
            const string password = "KQ1D8Wcm0M";
            const string secret = "SnyteunCeerhicJofI";

            var apiUrl = $"{baseUrl.TrimEnd('/')}/{loginPath.TrimStart('/')}";
            var form = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["grant_type"] = "client_credentials"
            };

            if (!string.IsNullOrWhiteSpace(secret))
            {
                form["secret"] = secret;
            }

            using var content = new FormUrlEncodedContent(form);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginResponse = await client.PostAsync(apiUrl, content);
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            Console.WriteLine($"POST {apiUrl} -> {(int)loginResponse.StatusCode} {loginResponse.ReasonPhrase}");
            if (!string.IsNullOrWhiteSpace(loginContent)) Console.WriteLine(loginContent);

            if (!loginResponse.IsSuccessStatusCode)
            {
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(loginContent);
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                return accessTokenElement.GetString();
            }

            return null;
        }
    }
}
