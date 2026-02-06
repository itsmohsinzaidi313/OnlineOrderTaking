using StackExchange.Redis;

namespace GatewayService
{
    public class ConnectionManager(IConnectionMultiplexer redis) : Interfaces.IConnectionManager
    {
        public async Task AddClientAsync(string clientId, string connectionId)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync(clientId + ":connection", connectionId);
        }

        public async Task<string?> GetConnectionIdAsync(string clientId)
        {
            var db = redis.GetDatabase();
            return await db.StringGetAsync(clientId + ":connection");
        }

        public async Task RemoveClientAsync(string clientId)
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(clientId + ":connection");
        }

        public async Task<bool> ClientExistsAsync(string clientId)
        {
            var db = redis.GetDatabase();
            return await db.KeyExistsAsync(clientId + ":connection");
        }
    }
}
