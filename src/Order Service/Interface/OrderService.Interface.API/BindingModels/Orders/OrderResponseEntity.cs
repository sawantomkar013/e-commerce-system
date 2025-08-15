namespace OrderService.Interface.API.BindingModels.Orders;

public record OrderResponseEntity
{
    public Guid OrderUuid { get; init; }

    public Guid CustomerUuid { get; init; }

    public Guid ProductUuid { get; init; }

    public int TotalQuantity { get; init; }

    public OrderStatus Status { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
