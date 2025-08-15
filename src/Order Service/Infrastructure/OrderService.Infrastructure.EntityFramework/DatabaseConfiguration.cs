using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.DataAccess;

namespace OrderService.Infrastructure.EntityFramework;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddOrderDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        return services;
    }
}
