using System.ComponentModel.DataAnnotations;

namespace OrderService.Interface.API.BindingModels.Orders;

public record CreateOrderRequestEntity : IValidatableObject
{
    [Required]
    public Guid? CustomerUuid { get; init; }

    [Required]
    public required ProductDetailsRequestEntity ProductDetails { get; init; }

    [Required]
    public OrderStatus? OrderStatus { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var logger = validationContext.GetService(typeof(ILogger<CreateOrderRequestEntity>)) as ILogger<CreateOrderRequestEntity>;
        if (OrderStatus == BindingModels.OrderStatus.Confirmed && ProductDetails.Quantity > 10)
        {
            logger?.LogWarning("Potential fraud detected: High-volume confirmed order.");
            yield return new ValidationResult(
                "Potential fraud detected: High-volume confirmed order.",
                [nameof(ProductDetails.Quantity)]);
        }
    }
}
