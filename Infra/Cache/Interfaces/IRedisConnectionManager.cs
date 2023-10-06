namespace Infra.Cache.Interfaces;

public interface IRedisConnectionManager
{
    Task SetKey(string key, string value, int expireInSeconds);
    Task<string> GetKey(string key);
    Task RemoveKey(string key);
}
