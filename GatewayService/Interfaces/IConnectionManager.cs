namespace GatewayService.Interfaces
{
    public interface IConnectionManager
    {
        public Task AddClientIdAsync(string clientId, string connectionId);
        public Task<string?> GetConnectionIdAsync(string clientId);
        public Task RemoveClientIdAsync(string clientId);
        public Task<bool> ClientIdExistsAsync(string clientId);
    }
}
