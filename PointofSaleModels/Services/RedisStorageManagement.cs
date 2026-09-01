using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PointofSaleModels.Services
{
    public class RedisStorageManagement(IConnectionMultiplexer connectionMultiplexer)
    {
        private IDatabase _database => connectionMultiplexer.GetDatabase();
        public async Task Store(string key, string value)
        {
            await _database.SetAddAsync(key, value);
        }

        public async Task<string> Retrieve(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? value.ToString() : throw new Exception();
        }

        public async Task Store<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value) ?? throw new Exception();
            await _database.SetAddAsync(key, json);
        }

        public async Task<T> Retrieve<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            var obj = JsonSerializer.Deserialize<T>(value.ToString()) ?? throw new Exception();
            return value.HasValue ? obj : throw new Exception();
        }
    }
}
