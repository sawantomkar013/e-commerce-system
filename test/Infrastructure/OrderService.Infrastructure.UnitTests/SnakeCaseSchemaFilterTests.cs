using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using OrderService.Infrastructure.NamingPolicy;

namespace OrderService.Infrastructure.UnitTests;

public class SnakeCaseSchemaFilterTests
{
    private readonly SnakeCaseSchemaFilter _filter = new();

    [Fact]
    public void Apply_ConvertsPropertiesToSnakeCase()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, OpenApiSchema>
            {
                { "FirstName", new OpenApiSchema { Type = "string" } },
                { "LastName", new OpenApiSchema { Type = "string" } },
                { "IPAddress", new OpenApiSchema { Type = "string" } },
            }
        };

        var context = new SchemaFilterContext(
            type: typeof(object),
            schemaGenerator: null,
            schemaRepository: null
        );

        // Act
        _filter.Apply(schema, context);

        // Assert
        Assert.Contains("first_name", schema.Properties.Keys);
        Assert.Contains("last_name", schema.Properties.Keys);
        Assert.Contains("i_p_address", schema.Properties.Keys);

        Assert.Equal(3, schema.Properties.Count);
    }

    [Fact]
    public void Apply_WithEmptyProperties_DoesNotThrow()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Properties = new Dictionary<string, OpenApiSchema>()
        };

        var context = new SchemaFilterContext(
            type: typeof(object),
            schemaGenerator: null,
            schemaRepository: null
        );

        // Act & Assert
        var exception = Record.Exception(() => _filter.Apply(schema, context));
        Assert.Null(exception);
    }

    [Fact]
    public void Apply_WithNullProperties_DoesNotThrow()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Properties = null
        };

        var context = new SchemaFilterContext(
            type: typeof(object),
            schemaGenerator: null,
            schemaRepository: null
        );

        // Act & Assert
        var exception = Record.Exception(() => _filter.Apply(schema, context));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("MyProperty", "my_property")]
    [InlineData("URL", "u_r_l")]
    [InlineData("Simple", "simple")]
    public void ToSnakeCase_PrivateMethod_WorksCorrectly(string input, string expected)
    {
        // Use reflection to invoke private method
        var method = typeof(SnakeCaseSchemaFilter)
            .GetMethod("ToSnakeCase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

        var result = method.Invoke(null, new object[] { input }) as string;

        Assert.Equal(expected, result);
    }
}
