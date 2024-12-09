using Microsoft.Extensions.Caching.Distributed;
using Offices.Services.Abstractions;
using System.Text.Json;

namespace Offices.Services.Services;

public class RedisCacheService : IRedisCahceService
{
    private readonly IDistributedCache? _cache;

    public RedisCacheService(IDistributedCache? cache)
    {
        _cache = cache;
    }

    public T GetCachedData<T>(string key)
    {
        var jsonData = _cache.GetString(key);

        if (jsonData == null)
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(jsonData)!;
    }

    public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration,

            //If we don't use data from cache for 2 minutes it will be removed
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        var jsonData = JsonSerializer.Serialize(data);

        _cache.SetString(key, jsonData, options);
    }
}
