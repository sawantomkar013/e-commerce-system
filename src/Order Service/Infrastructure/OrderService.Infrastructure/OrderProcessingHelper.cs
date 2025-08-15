namespace OrderService.Infrastructure;

using OrderService.Domain.DataAccess.Entities;
using System;

public static class OrderProcessingHelper
{
    public static string? GetKafkaTopicSuffix(OrderStatus status) =>
        status switch
        {
            OrderStatus.Pending => ".pending",
            OrderStatus.Confirmed => ".confirmed",
            OrderStatus.Shipped => ".shipped",
            _ => null
        };

    public static int GetRetryAttempts(OrderStatus status) =>
        status switch
        {
            OrderStatus.Pending => 3,
            OrderStatus.Confirmed => 5,
            OrderStatus.Shipped => 2,
            _ => 0
        };

    public static TimeSpan GetNotificationDelay(OrderStatus status) =>
        status switch
        {
            OrderStatus.Shipped => TimeSpan.FromSeconds(5),
            _ => TimeSpan.Zero
        };
}
