using Infra.Cache.Interfaces;
using StackExchange.Redis;

namespace Infra.Cache;

public class RedisConnectionManager : IRedisConnectionManager
{
    public readonly IDatabase DB;

    public RedisConnectionManager(string connectionString)
    {
        var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        DB = connectionMultiplexer.GetDatabase();
    }

    public async Task SetKey(string key, string value, int expireInSeconds) =>
        await DB.StringSetAsync(key, value, TimeSpan.FromSeconds(expireInSeconds));

    public async Task<string> GetKey(string key) => (await DB.StringGetAsync(key)).ToString() ?? string.Empty;

    public async Task RemoveKey(string key) => await DB.KeyDeleteAsync(key);
}
