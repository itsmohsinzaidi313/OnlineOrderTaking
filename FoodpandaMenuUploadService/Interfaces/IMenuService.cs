using System.Text.Json.Nodes;

namespace FoodpandaMenuUploadService.Interfaces
{
    public interface IMenuService
    {
        Task<JsonNode?> GetRestaurantMenu(int id);
    }
}