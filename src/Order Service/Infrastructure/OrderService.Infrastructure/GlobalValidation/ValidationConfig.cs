using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Infrastructure.GlobalValidation;
public static class ValidationConfig
{
    public static IServiceCollection AddCustomValidationResponses(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = string.IsNullOrEmpty(e.Key) ? null : e.Key,
                        Messages = e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
                    })
                    .ToArray();

                var problemDetails = new
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Code = "ValidationFailed",
                    Errors = errors
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            };
        });

        return services;
    }
}
