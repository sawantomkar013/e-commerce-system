using Xunit;
using OrderService.Domain.DataAccess.Entities;
using System;

public class OrderTests
{
    [Fact]
    public void Status_AuditTag_Mapping_Works()
    {
        var order = new Order { OrderId = "T1", Status = OrderStatus.Pending, TotalQuantity = 1, CreatedAt = DateTimeOffset.UtcNow };
        Assert.Equal("PENDING_FLOW", order.AuditTag);
    }
}
