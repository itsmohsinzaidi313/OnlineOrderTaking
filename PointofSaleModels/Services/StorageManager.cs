using PointofSaleModels.ServicePayloads;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text.Json;

namespace PointofSaleModels.Services
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
            await CacheObjectAsync($"{domainName}:{branchId}:menu", data);
        }

        public async Task CacheDeliveryAndPickupData(string domainName, string branchId, DataServicePayload data)
        {
            await CacheObjectAsync($"{domainName}:{branchId}:dandp", data);
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

        public async Task<bool> CacheObjectAsync<T>(string key, T data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            return await CacheStringAsync(key, serializedData);
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

        public async Task<bool> CacheStringAsync(string key, string data)
        {
            return await Db.StringSetAsync(key, data);
        }

        public async Task<string?> GetCachedStringAsync(string key)
        {
            return await Db.StringGetAsync(key);
        }

        public async Task<string> PublishForService<T>(T data)
        {
            var code = CreateCode();
            var saved = await CacheObjectAsync($"service:{code}", data);
            if (!saved)
            {
                throw new Exception("Failed to cache data for service communication.");
            }
            return code;
        }

        public async Task<T> GetServiceData<T>(string code)
        {
            var result = await GetCachedObject<T>($"service:{code}");
            return result == null ? throw new Exception("Service data not found.") : result;
        }

        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz1234567890";

        public static string CreateCode()
        {
            Span<char> buffer = stackalloc char[11];
            FillSegment(buffer[..5]);
            buffer[5] = '-';
            FillSegment(buffer[6..]);
            return new string(buffer);
        }

        private static void FillSegment(Span<char> target)
        {
            Span<byte> bytes = stackalloc byte[target.Length];
            RandomNumberGenerator.Fill(bytes);
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = Alphabet[bytes[i] % Alphabet.Length];
            }
        }
    }
}
