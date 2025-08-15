using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.DataAccess;

namespace OrderService.Infrastructure.EntityFramework.UnitTests;

public class DatabaseConfigurationTests
{
    [Fact]
    public void AddOrderDb_ShouldRegisterDbContextWithInterceptor()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<AuditSaveChangesInterceptor>();

        string connectionString = "DataSource=:memory:";

        // Act
        services.AddOrderDb(connectionString);
        var provider = services.BuildServiceProvider();

        var dbContext = provider.GetService<OrderDbContext>();
        var interceptor = provider.GetService<AuditSaveChangesInterceptor>();

        // Assert
        Assert.NotNull(dbContext);
        Assert.NotNull(interceptor);

        // Verify that the DbContextOptions contains our interceptor
        var options = provider.GetRequiredService<DbContextOptions<OrderDbContext>>();
        var hasInterceptor = options.Extensions
            .OfType<CoreOptionsExtension>()
            .Any(ext => ext.Interceptors.Contains(interceptor!));

        Assert.True(hasInterceptor);
    }

    [Fact]
    public void AddOrderDb_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<AuditSaveChangesInterceptor>();

        string connectionString = "DataSource=:memory:";

        // Act
        var result = services.AddOrderDb(connectionString);

        // Assert
        Assert.Same(services, result);
    }
}
