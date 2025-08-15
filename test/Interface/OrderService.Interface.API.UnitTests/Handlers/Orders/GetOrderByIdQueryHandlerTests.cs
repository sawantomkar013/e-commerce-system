using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.Commands.Orders;
using OrderService.Interface.API.Handlers.Orders;
using System.Text;
using System.Text.Json;

namespace OrderService.Interface.API.UnitTests.Handlers.Orders;

public class GetOrderByIdQueryHandlerTests
{
    private OrderDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrderDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderFromCache_WhenCacheExists()
    {
        // Arrange
        var db = GetDbContext();
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Confirmed,
            TotalQuantity = 5
        };
        await db.Orders.AddAsync(order);
        await db.SaveChangesAsync();

        var cacheMock = new Mock<IDistributedCache>();
        var cachedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
        cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(cachedBytes);

        var handler = new GetOrderByIdQueryHandler(db, cacheMock.Object);
        var request = new GetOrderByIdQuery(order.OrderId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.OrderId, result!.OrderId);

        cacheMock.Verify(c => c.GetAsync($"order:{order.OrderId}", It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderFromDbAndCache_WhenNotInCache()
    {
        // Arrange
        var db = GetDbContext();
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            TotalQuantity = 3
        };
        await db.Orders.AddAsync(order);
        await db.SaveChangesAsync();

        var cacheMock = new Mock<IDistributedCache>();
        cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);
        cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var handler = new GetOrderByIdQueryHandler(db, cacheMock.Object);
        var request = new GetOrderByIdQuery(order.OrderId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.OrderId, result!.OrderId);

        cacheMock.Verify(c => c.GetAsync($"order:{order.OrderId}", It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.SetAsync(
            $"order:{order.OrderId}",
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var db = GetDbContext();
        var cacheMock = new Mock<IDistributedCache>();
        cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);

        var handler = new GetOrderByIdQueryHandler(db, cacheMock.Object);
        var request = new GetOrderByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Null(result);

        cacheMock.Verify(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
