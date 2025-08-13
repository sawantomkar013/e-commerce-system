using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.DataAccess;

namespace OrderService.Infrastructure.EntityFramework
{
    public static class DbOptionsConfig
    {
        public static IServiceCollection AddOrderDb(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<OrderDbContext>(opt => opt.UseSqlite(connectionString));
            return services;
        }
    }
}
