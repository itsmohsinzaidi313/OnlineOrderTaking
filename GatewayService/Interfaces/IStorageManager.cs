
namespace GatewayService.Interfaces
{
    public interface IStorageManager
    {
        public Task SaveStringAsync(string key, string data);
        public Task<string?> GetStringAsync(string key);
        public Task DeleteDataAsync(string key);
        public Task<string?> GetPendingAndPop(string key);
        public Task PushToPending(string key, string data);
        public Task CacheMenuData(string domainName, int branchId, string data);
        public Task CacheDeliveryAndPickupData(string domainName, int branchId, string data);
        public Task<string?> GetCachedMenuData(string domainName, int branchId);
        public Task<string?> GetCachedDeliveryAndPickupData(string domainName, int branchId);
        public Task ClearMenuCache(string domainName, int branchId);
    }
}
