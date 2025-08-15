using OrderService.Interface.API.BindingModels;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands;
using OrderService.Interface.API.Commands.Orders;

namespace OrderService.Interface.API.UnitTests.Commands.Orders;

public class CreateOrderRequestTests
{
    [Fact]
    public void Constructor_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 5;
        var orderStatus = OrderStatus.Pending;

        var createOrderRequestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = customerId,
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = productId,
                Quantity = quantity
            },
            OrderStatus = orderStatus
        };

        // Act
        var request = new CreateOrderRequest(createOrderRequestEntity);

        // Assert
        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(productId, request.ProductId);
        Assert.Equal(quantity, request.Quantity);
        Assert.Equal(orderStatus.GetDomainOrderStatus(), request.OrderStatus);
    }

    [Fact]
    public void Constructor_ShouldHandleNullPropertiesGracefully()
    {
        // Arrange
        var createOrderRequestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = null,
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = null,
                Quantity = null
            },
            OrderStatus = null
        };

        // Act
        var request = new CreateOrderRequest(createOrderRequestEntity);

        // Assert: All default values are set
        Assert.Equal(Guid.Empty, request.CustomerId);
        Assert.Equal(Guid.Empty, request.ProductId);
        Assert.Equal(0, request.Quantity);
        Assert.Equal(default, request.OrderStatus);
    }
}