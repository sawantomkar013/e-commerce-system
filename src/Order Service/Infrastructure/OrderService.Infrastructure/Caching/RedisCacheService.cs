using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace OrderService.Infrastructure.Caching;

public class RedisCacheService(
    IDistributedCache cache,
    IOptions<RedisSettings> settings,
    ILogger<RedisCacheService> logger) : IRedisCacheService
{
    private readonly RedisSettings _settings = settings.Value;

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.AbsoluteExpirationMinutes)
            };

            var jsonData = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, jsonData, options, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set cache for key: {Key}", key);
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonData = await cache.GetStringAsync(key, cancellationToken);

            if (jsonData is null)
                return default;

            return JsonSerializer.Deserialize<T>(jsonData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get cache for key: {Key}", key);
            return default;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove cache for key: {Key}", key);
        }
    }
}