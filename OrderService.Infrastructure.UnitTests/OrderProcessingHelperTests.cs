using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.UnitTests;

public class OrderProcessingHelperTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, ".pending")]
    [InlineData(OrderStatus.Confirmed, ".confirmed")]
    [InlineData(OrderStatus.Shipped, ".shipped")]
    [InlineData((OrderStatus)999, null)] // Unknown status
    public void GetKafkaTopicSuffix_ReturnsExpectedSuffix(OrderStatus status, string? expected)
    {
        // Act
        var result = OrderProcessingHelper.GetKafkaTopicSuffix(status);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, 3)]
    [InlineData(OrderStatus.Confirmed, 5)]
    [InlineData(OrderStatus.Shipped, 2)]
    [InlineData((OrderStatus)999, 0)] // Unknown status
    public void GetRetryAttempts_ReturnsExpectedAttempts(OrderStatus status, int expected)
    {
        // Act
        var result = OrderProcessingHelper.GetRetryAttempts(status);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(OrderStatus.Shipped, 5)] // seconds
    [InlineData(OrderStatus.Pending, 0)]
    [InlineData(OrderStatus.Confirmed, 0)]
    [InlineData((OrderStatus)999, 0)] // Unknown status
    public void GetNotificationDelay_ReturnsExpectedDelay(OrderStatus status, int expectedSeconds)
    {
        // Act
        var result = OrderProcessingHelper.GetNotificationDelay(status);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), result);
    }
}