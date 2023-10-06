using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Newtonsoft.Json;

namespace Infra.Cache;

public abstract class BaseRedisCacheRepository
{
    private readonly IRedisConnectionManager _redisConnection;
    private readonly CacheOptions _cacheOptions;
    private readonly string prefixKey;

    protected BaseRedisCacheRepository(
        IRedisConnectionManager redisConnection,
        CacheOptions cacheOptions,
        string serviceId,
        string entityName
        )
    {
        _redisConnection = redisConnection;
        _cacheOptions = cacheOptions;
        prefixKey = $"{serviceId}:{entityName}";
    }

    protected async Task<T> GetOrRefreshCache<T>(string key, Func<Task<T>> repositoryOperation, string? userScope = null)
    {
        T cacheResult = await TryGetFromCache<T>(key, userScope);

        if (EqualityComparer<T>.Default.Equals(cacheResult, default))
        {
            cacheResult = await repositoryOperation();

            if (cacheResult is null)
                return cacheResult;

            await SetCacheByKey(key, userScope, cacheResult);
        }

        return cacheResult;
    }

    protected async Task<T> TryGetFromCache<T>(string key, string? userScope = null) =>
        !string.IsNullOrWhiteSpace(userScope) ? await GetCacheByKey<T>(key, userScope) : await GetCacheByKey<T>(key);

    protected async Task<T> GetCacheByKey<T>(string key) =>
        await GetCacheByKey<T>(key, string.Empty);

    protected async Task<T> GetCacheByKey<T>(string key, string? scope)
    {
        var fullKey = BuildKey(key, scope);
        var cache = await _redisConnection.GetKey(fullKey);

        if (string.IsNullOrEmpty(cache))
            return default;

        var jsonSerializerSettings = new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        return JsonConvert.DeserializeObject<T>(cache, jsonSerializerSettings)!;
    }

    protected async Task SetCacheByKey<T>(string key, string? scope, T value)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            return;

        var fullKey = BuildKey(key, scope);
        var cache = JsonConvert.SerializeObject(value);

        await _redisConnection.SetKey(fullKey, cache, _cacheOptions.ExpirationSeconds);
    }

    private string BuildKey(string key, string? scope) =>
        string.IsNullOrWhiteSpace(scope) ? $"{prefixKey}:{key}" : $"{prefixKey}:{scope}:{key}";
}
