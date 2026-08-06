namespace FoodpandaMenuUploadService.Interfaces
{
    public interface IUploadService
    {
        Task<string> Initiate(System.Text.Json.Nodes.JsonNode menu);
    }
}