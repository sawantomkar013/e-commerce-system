using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderService.Infrastructure.GlobalValidation;

namespace OrderService.Infrastructure.UnitTests;
public class GlobalValidationExtensionsTests
{
    [Fact]
    public void AddCustomValidationResponses_SetsInvalidModelStateResponseFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCustomValidationResponses();
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;

        Assert.NotNull(options.InvalidModelStateResponseFactory);

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Name is required");

        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(
            httpContext: httpContext,
            routeData: new Microsoft.AspNetCore.Routing.RouteData(),
            actionDescriptor: new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            modelState: modelState
        );

        // Act
        var result = options.InvalidModelStateResponseFactory(actionContext) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result!.StatusCode);

        var problemDetails = result!.Value!;
        var codeProperty = problemDetails.GetType().GetProperty("Code")!.GetValue(problemDetails);
        Assert.Equal("ValidationFailed", codeProperty);
        var errorsProperty = problemDetails.GetType().GetProperty("Errors")!.GetValue(problemDetails) as Array;
        Assert.NotNull(errorsProperty);
        Assert.Single(errorsProperty!);

        var firstError = errorsProperty.GetValue(0)!;
        var fieldProperty = firstError.GetType().GetProperty("Field")!.GetValue(firstError);
        var messagesProperty = firstError.GetType().GetProperty("Messages")!.GetValue(firstError) as string[];

        Assert.Equal("Name", fieldProperty);
        Assert.Contains("Name is required", messagesProperty!);
    }
}

