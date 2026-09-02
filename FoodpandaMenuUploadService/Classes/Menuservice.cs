using System.Text.Json.Nodes;
using FoodpandaMenuUploadService.Interfaces;

namespace FoodpandaMenuUploadService.Classes
{
    public class MenuService(IConfiguration configuration) : IMenuService
    {
        public async Task<JsonNode?> GetRestaurantMenu(int id)
        {
            var url = configuration["MenuUrl"] ?? throw new InvalidOperationException("MenuUrl is not configured.");
            var httpClient = new HttpClient();
            var body = new
            {
                OperationId = 1,
                CompanyId = id,
                OrderSourceValue = "WEB"
            };
            var content = JsonContent.Create(body);
            var response = await httpClient.PostAsync(url, content);
            var ygenJson = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(ygenJson);
        }
    }
}