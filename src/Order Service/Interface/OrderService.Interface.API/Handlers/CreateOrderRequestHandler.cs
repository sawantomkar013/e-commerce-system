using MediatR;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.BackgroundJobs;
using OrderService.Infrastructure.ServiceClients;
using OrderService.Interface.API.Commands.Orders;
using System.Text.Json;
using CreateOrderResponse = OrderService.Interface.API.Commands.Response<
    OrderService.Domain.DataAccess.Entities.Order,
    OrderService.Interface.API.Commands.Orders.CreateOrderRequest.Errors>;

namespace OrderService.Interface.API.Handlers;

public class CreateOrderRequestHandler(
    OrderDbContext dbContext,
    IKafkaProducer kafka,
    INotificationClient notificationService,
    ILogger<CreateOrderRequestHandler> logger) : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            TotalQuantity = request.Quantity,
            Status = request.OrderStatus,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Save order
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (order.Status != OrderStatus.Cancelled)
        {
            var notifyPayload = new
            {
                orderId = order.OrderId,
                status = order.Status.ToString(),
                totalQuantity = order.TotalQuantity,
                createdAt = order.CreatedAt
            };
            var notificationResult = await notificationService.SendAsync(order.Status, notifyPayload, cancellationToken);
            if (!notificationResult.Success)
            {
                logger.LogError("Notification Servie failed due to Error : {message}", notificationResult.Message);
            }

            var payload = JsonSerializer.Serialize(new { orderId = order.OrderId, timestamp = order.CreatedAt });
            var result = await kafka.ProduceAsync(order.Status, order.OrderId.ToString(), payload, cancellationToken);
            if (!result.Success)
            {
                logger.LogError("Kafka failed due to Error : {errorReason}", result.ErrorReason);
            }
        }

        return CreateOrderResponse.Ok(order);
    }
}
