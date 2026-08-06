using System.Net.Http.Headers;
using FoodpandaMenuUploadService.Interfaces;

namespace FoodpandaMenuUploadService.Classes
{
    public class AccessToken : IAccessToken
    {
        private System.Text.Json.Nodes.JsonObject AccessTokenJson { get; set; }
        public async Task<string> GetTokenAsync()
        {
            var token = AccessTokenJson?["access_token"]?.GetValue<string>();
            if(token == null)
            {
                await FetchAccessToken();
                token = AccessTokenJson?["access_token"]?.GetValue<string>();
            }
            return token ?? throw new Exception("Access token is not available.");
        }

        private async Task FetchAccessToken()
        {
            var accessToken = AccessTokenJson?["access_token"]?.GetValue<string>();
            var expiresIn = AccessTokenJson?["expires_in"]?.GetValue<int>();
            if (accessToken != null && expiresIn != null)
            {
                var expirationTime = DateTime.UtcNow.AddSeconds(expiresIn.Value);
                if (DateTime.UtcNow < expirationTime) return;
            }
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
                ["grant_type"] = "client_credentials",
                ["secret"] = secret
            };

            using var content = new FormUrlEncodedContent(form);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginResponse = await client.PostAsync(apiUrl, content);
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            if (!loginResponse.IsSuccessStatusCode) return;

            AccessTokenJson = System.Text.Json.Nodes.JsonNode.Parse(loginContent)?.AsObject() ?? throw new Exception("Failed to parse access token response.");

            return;
        }
    }
}