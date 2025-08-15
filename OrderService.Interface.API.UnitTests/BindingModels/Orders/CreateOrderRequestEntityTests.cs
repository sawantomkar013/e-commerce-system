using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Interface.API.BindingModels;
using OrderService.Interface.API.BindingModels.Orders;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Interface.API.UnitTests.BindingModels.Orders;

public class CreateOrderRequestEntityTests
{
    private ValidationContext CreateValidationContext(CreateOrderRequestEntity entity, ILogger<CreateOrderRequestEntity>? logger = null)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<CreateOrderRequestEntity>)))
                       .Returns(logger);
        return new ValidationContext(entity, serviceProvider.Object, null);
    }

    [Fact]
    public void Validate_ShouldReturnRequiredValidationErrors_WhenPropertiesAreMissing()
    {
        // Arrange
        var entity = new CreateOrderRequestEntity
        {
            CustomerUuid = null,
            ProductDetails = new ProductDetailsRequestEntity { Uuid = Guid.NewGuid(), Quantity = 5 },
            OrderStatus = null
        };

        // Act
        var results = new List<ValidationResult>();
        var context = new ValidationContext(entity, serviceProvider: null, items: null);
        bool isValid = Validator.TryValidateObject(entity, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(entity.CustomerUuid)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(entity.OrderStatus)));
    }

    [Fact]
    public void Validate_ShouldReturnFraudWarning_WhenConfirmedOrderQuantityHigh()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateOrderRequestEntity>>();
        var entity = new CreateOrderRequestEntity
        {
            CustomerUuid = Guid.NewGuid(),
            ProductDetails = new ProductDetailsRequestEntity { Quantity = 20, Uuid = Guid.NewGuid() },
            OrderStatus = OrderStatus.Confirmed
        };

        // Act
        var results = entity.Validate(CreateValidationContext(entity, loggerMock.Object));

        // Assert
        var resultList = new List<ValidationResult>(results);
        Assert.Single(resultList);
        Assert.Equal("Potential fraud detected: High-volume confirmed order.", resultList[0].ErrorMessage);
        Assert.Contains(nameof(entity.ProductDetails.Quantity), resultList[0].MemberNames);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Potential fraud detected")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Validate_ShouldNotReturnFraudWarning_WhenConfirmedOrderQuantityLow()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateOrderRequestEntity>>();
        var entity = new CreateOrderRequestEntity
        {
            CustomerUuid = Guid.NewGuid(),
            ProductDetails = new ProductDetailsRequestEntity { Quantity = 5, Uuid = Guid.NewGuid() },
            OrderStatus = OrderStatus.Confirmed
        };

        // Act
        var results = entity.Validate(CreateValidationContext(entity, loggerMock.Object));

        // Assert
        Assert.Empty(results);
        loggerMock.Verify(
            x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception?, string>>()),
            Times.Never);
    }
}
