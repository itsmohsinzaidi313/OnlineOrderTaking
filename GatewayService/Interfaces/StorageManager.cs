
using StackExchange.Redis;

namespace GatewayService.Interfaces
{
    public class StorageManager(IConnectionMultiplexer redis) : IStorageManager
    {
        public async Task DeleteDataAsync(string key)
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task<string?> GetPendingAndPop(string key)
        {
            var db = redis.GetDatabase();
            return await db.ListRightPopAsync("pending:" + key);

        }

        public async Task PushToPending(string key, string data)
        {
            var db = redis.GetDatabase();
            await db.ListLeftPushAsync("pending:" + key, data);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var db = redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SaveStringAsync(string key, string data)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync(key, data);
        }

        public async Task CacheMenuData(string domainName, int branchId, string data)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync($"{domainName}:{branchId}:menu", data);
        }

        public async Task CacheDeliveryAndPickupData(string domainName, int branchId, string data)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync($"{domainName}:{branchId}:dandp", data);
        }

        public async Task<string?> GetCachedMenuData(string domainName, int branchId)
        {
            var db = redis.GetDatabase();
            return await db.StringGetAsync($"{domainName}:{branchId}:menu");
        }

        public async Task<string?> GetCachedDeliveryAndPickupData(string domainName, int branchId)
        {
            var db = redis.GetDatabase();
            return await db.StringGetAsync($"{domainName}:{branchId}:dandp");
        }

        public Task ClearMenuCache(string domainName, int branchId)
        {
            throw new NotImplementedException();
        }
    }
}
