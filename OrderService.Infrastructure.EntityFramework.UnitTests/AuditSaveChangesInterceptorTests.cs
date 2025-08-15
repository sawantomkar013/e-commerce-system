using Microsoft.EntityFrameworkCore;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.EntityFramework.UnitTests;

public class AuditSaveChangesInterceptorTests
{
    private class TestDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    private DbContextOptions<TestDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditSaveChangesInterceptor())
            .Options;
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldAddAuditLog_WhenOrderAdded()
    {
        // Arrange
        var options = CreateOptions();
        await using var context = new TestDbContext(options);

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Pending
        };
        context.Orders.Add(order);

        // Act
        await context.SaveChangesAsync();

        // Assert
        var auditLog = context.AuditLogs.SingleOrDefault();
        Assert.NotNull(auditLog);
        Assert.Equal(order.OrderId.ToString(), auditLog!.OrderId);
        Assert.Equal("PENDING_FLOW", auditLog.Tag);
        Assert.Contains("created", auditLog.Message);
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldAddAuditLog_WhenOrderModified()
    {
        // Arrange
        var options = CreateOptions();
        await using var context = new TestDbContext(options);

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Pending
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Modify order
        order.Status = OrderStatus.Confirmed;
        context.Orders.Update(order);

        // Act
        await context.SaveChangesAsync();

        // Assert
        var auditLog = context.AuditLogs.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
        Assert.NotNull(auditLog);
        Assert.Equal(order.OrderId.ToString(), auditLog!.OrderId);
        Assert.Equal("CONFIRMED_FLOW", auditLog.Tag);
        Assert.Contains("updated", auditLog.Message);
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldNotAddAuditLog_WhenNoRelevantChanges()
    {
        // Arrange
        var options = CreateOptions();
        await using var context = new TestDbContext(options);

        // Act
        await context.SaveChangesAsync();

        // Assert
        Assert.Empty(context.AuditLogs);
    }
}
