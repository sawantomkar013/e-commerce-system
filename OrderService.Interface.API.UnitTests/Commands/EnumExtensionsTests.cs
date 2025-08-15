using OrderService.Interface.API.BindingModels;
using OrderService.Interface.API.Commands;
using System.ComponentModel;

namespace OrderService.Interface.API.UnitTests.Commands;

public class EnumsExtensionsTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, Domain.DataAccess.Entities.OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed, Domain.DataAccess.Entities.OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped, Domain.DataAccess.Entities.OrderStatus.Shipped)]
    [InlineData(OrderStatus.Cancelled, Domain.DataAccess.Entities.OrderStatus.Cancelled)]
    public void GetDomainOrderStatus_ShouldMapCorrectly(OrderStatus input, Domain.DataAccess.Entities.OrderStatus expected)
    {
        // Act
        var result = input.GetDomainOrderStatus();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetDomainOrderStatus_ShouldThrowException_ForInvalidValue()
    {
        // Arrange
        var invalidValue = (OrderStatus)999;

        // Act & Assert
        Assert.Throws<InvalidEnumArgumentException>(() => invalidValue.GetDomainOrderStatus());
    }
}
