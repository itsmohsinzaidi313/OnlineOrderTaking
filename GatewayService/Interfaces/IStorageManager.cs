
namespace GatewayService.Interfaces
{
    public interface IStorageManager
    {
        public Task DeleteAsync(string key);
        public Task<string?> GetCachedStringAsync(string key);
        public Task CacheStringAsync(string key, string data);
        public Task ClearCacheByPattern(string pattern);
        public Task CacheObject<T>(string key, T data);
        public Task<T?> GetCachedObject<T>(string key);
        public Task<string?> GetPendingAndPop(string key);
        public Task PushToPending(string key, string data);
    }
}
