using System.Text.Json.Nodes;

namespace FoodpandaMenuUploadService.Interfaces
{
    public interface IFoodPandaTransformer
    {
        public JsonObject Transform(JsonNode source);
    }
}