using MediatR;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.BindingModels.Orders;

namespace OrderService.Interface.API.Commands.Orders;

public record CreateOrderRequest : IRequest<Response<Order, CreateOrderRequest.Errors>> 
{
    public CreateOrderRequest(CreateOrderRequestEntity createOrderRequestEntity)
    {
        CustomerId = createOrderRequestEntity.CustomerUuid.GetValueOrDefault();
        ProductId = createOrderRequestEntity.ProductDetails.Uuid.GetValueOrDefault();
        Quantity = createOrderRequestEntity.ProductDetails.Quantity.GetValueOrDefault();
        OrderStatus = createOrderRequestEntity.OrderStatus.GetValueOrDefault().GetDomainOrderStatus();
    }

    public Guid CustomerId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public enum Errors
    {
        Unknown,
    }
}
