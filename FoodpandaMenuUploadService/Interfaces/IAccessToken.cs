namespace FoodpandaMenuUploadService.Interfaces
{
    public interface IAccessToken
    {
        Task<string> GetTokenAsync();
    }
}