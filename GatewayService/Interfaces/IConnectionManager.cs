namespace GatewayService.Interfaces
{
    public interface IConnectionManager
    {
        public Task AddClientAsync(string clientId, string connectionId);
        public Task<string?> GetConnectionIdAsync(string clientId);
        public Task RemoveClientAsync(string clientId);
        public Task<bool> ClientExistsAsync(string clientId);
    }
}
