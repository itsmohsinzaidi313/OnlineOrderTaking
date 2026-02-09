using GatewayService.Interfaces;
using PointofSaleModels.Services;
using StackExchange.Redis;

namespace GatewayService.Classes
{
    public class ConnectionManager(IStorageManager storage) : Interfaces.IConnectionManager
    {
        private const string ConnectionKeySuffix = ":connection";
        public async Task AddClientIdAsync(string clientId, string connectionId)
        {
            await storage.CacheStringAsync(clientId + ConnectionKeySuffix, connectionId);
        }

        public async Task<string?> GetConnectionIdAsync(string clientId)
        {
            return await storage.GetCachedStringAsync(clientId + ConnectionKeySuffix);
        }

        public async Task RemoveClientIdAsync(string clientId)
        {
            await storage.DeleteAsync(clientId + ConnectionKeySuffix);
        }

        public async Task<bool> ClientIdExistsAsync(string clientId)
        {
            return await storage.GetCachedStringAsync(clientId + ConnectionKeySuffix) != null;
        }
    }
}
