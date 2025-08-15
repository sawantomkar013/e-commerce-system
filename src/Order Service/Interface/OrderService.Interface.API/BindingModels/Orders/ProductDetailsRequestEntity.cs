using System.ComponentModel.DataAnnotations;

namespace OrderService.Interface.API.BindingModels.Orders;

public record ProductDetailsRequestEntity
{
    [Required]
    public Guid? Uuid { get; init; }

    [Required]
    public int? Quantity { get; init; }
}
