using OrderService.Interface.API.Commands.Orders;

namespace OrderService.Interface.API.UnitTests.Commands.Orders;

public class GetOrderByIdQueryTests
{
    [Fact]
    public void Constructor_ShouldSetOrderId()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var query = new GetOrderByIdQuery(orderId);

        // Assert
        Assert.Equal(orderId, query.OrderId);
    }
}
