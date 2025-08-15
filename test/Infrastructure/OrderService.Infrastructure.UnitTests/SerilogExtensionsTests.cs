using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using OrderService.Infrastructure.Logging;

namespace OrderService.Infrastructure.UnitTests;
public class SerilogExtensionsTests
{
    [Fact]
    public void ConfigureSerilog_ShouldNotThrow()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Array.Empty<string>(),
            ApplicationName = "TestApp"
        });
        builder.Configuration.AddConfiguration(configuration);

        // Act & Assert
        var exception = Record.Exception(() => builder.ConfigureSerilog());
        Assert.Null(exception);
    }
}
