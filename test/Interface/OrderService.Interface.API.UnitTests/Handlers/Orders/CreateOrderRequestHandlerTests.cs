using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.BackgroundJobs;
using OrderService.Infrastructure.ServiceClients;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands.Orders;
using OrderService.Interface.API.Handlers.Orders;

namespace OrderService.Interface.API.UnitTests.Handlers.Orders;

public class CreateOrderRequestHandlerTests
{
    private OrderDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldSaveOrder_AndSendNotification_AndProduceKafka()
    {
        // Arrange
        var dbContext = GetDbContext();

        var kafkaMock = new Mock<IKafkaProducer>();
        kafkaMock.Setup(k => k.ProduceAsync(It.IsAny<OrderStatus>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new KafkaProducerResult { Success = true });

        var notificationMock = new Mock<INotificationClient>();
        notificationMock.Setup(n => n.SendAsync(It.IsAny<OrderStatus>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new NotificationResult { Success = true });

        var loggerMock = new Mock<ILogger<CreateOrderRequestHandler>>();

        var handler = new CreateOrderRequestHandler(dbContext, kafkaMock.Object, notificationMock.Object, loggerMock.Object);

        var requestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = Guid.NewGuid(),
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = Guid.NewGuid(),
                Quantity = 5
            },
            OrderStatus = (API.BindingModels.OrderStatus?)OrderStatus.Confirmed
        };

        var request = new CreateOrderRequest(requestEntity);
        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        // Order saved
        var savedOrder = await dbContext.Orders.FirstOrDefaultAsync();
        Assert.NotNull(savedOrder);
        Assert.Equal(request.CustomerId, savedOrder.CustomerId);
        Assert.Equal(request.ProductId, savedOrder.ProductId);
        Assert.Equal(request.Quantity, savedOrder.TotalQuantity);
        Assert.Equal(request.OrderStatus, savedOrder.Status);

        // Notification sent
        notificationMock.Verify(n => n.SendAsync(savedOrder.Status, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);

        // Kafka message produced
        kafkaMock.Verify(k => k.ProduceAsync(savedOrder.Status, savedOrder.OrderId.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        // Response contains the correct OrderUuid
        Assert.Equal(savedOrder.OrderId, response.Value.OrderId);
    }

    [Fact]
    public async Task Handle_ShouldNotNotifyOrProduce_WhenOrderCancelled()
    {
        // Arrange
        var dbContext = GetDbContext();

        var kafkaMock = new Mock<IKafkaProducer>();
        var notificationMock = new Mock<INotificationClient>();
        var loggerMock = new Mock<ILogger<CreateOrderRequestHandler>>();

        var handler = new CreateOrderRequestHandler(dbContext, kafkaMock.Object, notificationMock.Object, loggerMock.Object);


        var requestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = Guid.NewGuid(),
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = Guid.NewGuid(),
                Quantity = 5
            },
            OrderStatus = (API.BindingModels.OrderStatus?)OrderStatus.Cancelled,
        };

        var request = new CreateOrderRequest(requestEntity);

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        var savedOrder = await dbContext.Orders.FirstOrDefaultAsync();
        Assert.NotNull(savedOrder);
        Assert.Equal(OrderStatus.Cancelled, savedOrder.Status);

        notificationMock.Verify(n => n.SendAsync(It.IsAny<OrderStatus>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        kafkaMock.Verify(k => k.ProduceAsync(It.IsAny<OrderStatus>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLogErrors_WhenNotificationFails()
    {
        // Arrange
        var dbContext = GetDbContext();

        var kafkaMock = new Mock<IKafkaProducer>();
        kafkaMock.Setup(k => k.ProduceAsync(It.IsAny<OrderStatus>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new KafkaProducerResult { Success = true });

        var notificationMock = new Mock<INotificationClient>();
        notificationMock.Setup(n => n.SendAsync(It.IsAny<OrderStatus>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new NotificationResult { Success = false, Message = "Fail" });

        var loggerMock = new Mock<ILogger<CreateOrderRequestHandler>>();

        var handler = new CreateOrderRequestHandler(dbContext, kafkaMock.Object, notificationMock.Object, loggerMock.Object);

        var requestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = Guid.NewGuid(),
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = Guid.NewGuid(),
                Quantity = 5
            },
            OrderStatus = (API.BindingModels.OrderStatus?)OrderStatus.Confirmed
        };

        var request = new CreateOrderRequest(requestEntity);

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Notification Servie failed due to Error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );
    }
}
