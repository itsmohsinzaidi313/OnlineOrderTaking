namespace PointofSaleModels.Services
{
    public interface IStorageManager
    {
        public Task DeleteAsync(string key);
        public Task<string?> GetCachedStringAsync(string key);
        public Task<bool> CacheStringAsync(string key, string data);
        public Task ClearCacheByPattern(string pattern);
        public Task<bool> CacheObjectAsync<T>(string key, T data);
        public Task<T?> GetCachedObject<T>(string key);
        public Task<string?> GetPendingAndPop(string key);
        public Task PushToPending(string clientId, string data);
        public Task<string> PublishForService<T>(T data);
        public Task<T> GetServiceData<T>(string code);
    }
}
