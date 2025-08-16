using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands;
using OrderService.Interface.API.Commands.Orders;
using OrderService.Interface.API.Controllers.Orders;
using OrderService.Interface.API.Mappers.Orders;

namespace OrderService.Interface.API.UnitTests.Controllers.Orders;

public class OrdersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly OrderMapper _mapper = new();

    private OrdersController CreateController() =>
        new OrdersController(_mediatorMock.Object, _cacheMock.Object, _mapper);

    [Fact]
    public async Task Create_ShouldReturnCreatedResult_AndCacheOrder()
    {
        // Arrange
        var controller = CreateController();
        var orderId = Guid.NewGuid();
        var orderEntity = new Order
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            TotalQuantity = 2,
            Status = OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createRequestEntity = new CreateOrderRequestEntity
        {
            CustomerUuid = orderEntity.CustomerId,
            ProductDetails = new ProductDetailsRequestEntity
            {
                Uuid = orderEntity.ProductId,
                Quantity = orderEntity.TotalQuantity
            },
            OrderStatus = (API.BindingModels.OrderStatus?)orderEntity.Status
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response<Order, CreateOrderRequest.Errors>.Ok(orderEntity));

        _cacheMock
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.Create(createRequestEntity, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(controller.GetById), createdResult.ActionName);

        var mappedResponse = _mapper.OrderToOrderResponseEntity(orderEntity);
        Assert.Equal(mappedResponse.OrderUuid, ((OrderResponseEntity)createdResult.Value!).OrderUuid);

        _cacheMock.Verify(c => c.SetAsync(
            $"order:{orderId}",
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenOrderExists()
    {
        // Arrange
        var controller = CreateController();
        var orderId = Guid.NewGuid();
        var orderEntity = new Order { OrderId = orderId, Status = OrderStatus.Confirmed };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderEntity);

        // Act
        var result = await controller.GetById(orderId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mappedResponse = _mapper.OrderToOrderResponseEntity(orderEntity);
        Assert.Equal(mappedResponse.OrderUuid, ((OrderResponseEntity)okResult.Value!).OrderUuid);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var controller = CreateController();
        var orderId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await controller.GetById(orderId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}