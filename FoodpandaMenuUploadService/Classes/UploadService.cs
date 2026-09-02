using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using FoodpandaMenuUploadService.Interfaces;

namespace FoodpandaMenuUploadService.Classes
{
    public class UploadService(IAccessToken accessTokenService, IConfiguration configuration) : IUploadService
    {
        public async Task<string> Initiate(JsonNode menu)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var content = JsonContent.Create(menu, options: options);
            var httpClient = new HttpClient();
            var accessToken = await accessTokenService.GetTokenAsync();
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var catalogUrl = configuration["CatalogUrl"] ?? throw new InvalidOperationException("CatalogUrl is not configured.");
            var response = await httpClient.PutAsync(catalogUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}