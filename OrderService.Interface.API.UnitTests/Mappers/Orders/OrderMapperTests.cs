using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.Mappers.Orders;

namespace OrderService.Interface.API.UnitTests.Mappers.Orders;

public class OrderMapperTests
{
    private readonly OrderMapper _mapper = new OrderMapper();

    [Fact]
    public void OrderToOrderResponseEntity_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            TotalQuantity = 5,
            Status = OrderStatus.Confirmed,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = _mapper.OrderToOrderResponseEntity(order);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.OrderId, result.OrderUuid);
        Assert.Equal(order.CustomerId, result.CustomerUuid);
        Assert.Equal(order.ProductId, result.ProductUuid);
        Assert.Equal(order.TotalQuantity, result.TotalQuantity);
        Assert.Equal(order.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public void OrderToOrderResponseEntity_ShouldThrowOnNullInput()
    {
        // Arrange
        Order? order = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _mapper.OrderToOrderResponseEntity(order!));
    }
}
