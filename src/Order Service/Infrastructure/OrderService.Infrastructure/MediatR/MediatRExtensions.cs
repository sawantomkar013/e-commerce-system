using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Infrastructure.MediatRSetup
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddMediatRAndBehaviors(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            return services;
        }
    }
}
