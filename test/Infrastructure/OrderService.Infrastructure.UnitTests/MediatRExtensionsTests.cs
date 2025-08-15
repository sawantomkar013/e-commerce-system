using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Infrastructure.MediatR;

namespace OrderService.Infrastructure.UnitTests;
public class MediatRExtensionsTests
{
    [Fact]
    public void AddMediatRAndBehaviors_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Add logging so MediatR can resolve ILoggerFactory
        services.AddLogging();

        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var returnedServices = services.AddMediatRAndBehaviors(assembly);

        // Assert
        Assert.NotNull(returnedServices);
        Assert.Contains(returnedServices, s => s.ServiceType == typeof(IMediator));

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        Assert.NotNull(mediator);
    }
}

