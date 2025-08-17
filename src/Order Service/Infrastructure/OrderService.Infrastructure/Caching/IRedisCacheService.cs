namespace OrderService.Infrastructure.Caching;

public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

    Task RemoveAsync(string key, CancellationToken cancellationToken);
}
