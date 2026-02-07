using GatewayService.Interfaces;
using PointofSaleModels.ServicePayloads;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.Models
{
    public class StorageManager(IConnectionMultiplexer redis) : IStorageManager
    {
        public IDatabase Db => redis.GetDatabase();
        public IServer Server => redis.GetServer(redis.GetEndPoints().First());
        public async Task DeleteAsync(string key)
        {
            await Db.KeyDeleteAsync(key);
        }

        public async Task<string?> GetPendingAndPop(string key)
        {
            return await Db.ListRightPopAsync("pending:" + key);
        }

        public async Task PushToPending(string key, string data)
        {
            await Db.ListLeftPushAsync("pending:" + key, data);
        }

        public async Task CacheMenuData(string domainName, string branchId, DataServicePayload data)
        {
            await CacheObject($"{domainName}:{branchId}:menu", data);
        }

        public async Task CacheDeliveryAndPickupData(string domainName, string branchId, DataServicePayload data)
        {
            await CacheObject($"{domainName}:{branchId}:dandp", data);
        }

        public async Task<DataServicePayload?> GetCachedMenuData(string domainName, string branchId)
        {
            return await GetCachedObject<DataServicePayload>($"{domainName}:{branchId}:menu");
        }

        public async Task<DataServicePayload?> GetCachedDeliveryAndPickupData(string domainName, string branchId)
        {
            return await GetCachedObject<DataServicePayload>($"{domainName}:{branchId}:dandp");
        }

        public async Task ClearMenuCache(string domainName = "*", string branchId = "*")
        {
            await ClearCacheByPattern($"{domainName}:{branchId}:menu");
        }

        public async Task ClearDeliveryAndPickupCache(string domainName = "*", string branchId = "*")
        {
            await ClearCacheByPattern($"{domainName}:{branchId}:dandp");
        }

        public async Task CacheObject<T>(string key, T data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            await CacheStringAsync(key, serializedData);
        }

        public async Task<T?> GetCachedObject<T>(string key)
        {
            var serializedData = await GetCachedStringAsync(key);
            if (string.IsNullOrEmpty(serializedData))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(serializedData);
        }

        public async Task ClearCacheByPattern(string pattern)
        {
            await foreach (var key in Server.KeysAsync(pattern: pattern))
            {
                await Db.KeyDeleteAsync(key);
            }
        }

        public async Task CacheStringAsync(string key, string data)
        {
            var isSaved = await Db.StringSetAsync(key, data);
            if(!isSaved)
            {
                throw new Exception("Failed to save data to Redis.");
            }
        }

        public async Task<string?> GetCachedStringAsync(string key)
        {
            return await GetStringAsync(key);
        }
    }
}
