using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace OrderService.Infrastructure.Caching
{
    public static class DistributedCachingExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, string connection)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connection));
            return services;
        }
    }
}
