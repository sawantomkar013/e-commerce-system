using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace OrderService.Infrastructure.Logging
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog(logger, dispose: true);
        }
    }
}
