using MediatR;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Interface.API.Handlers
{
    public record CreateOrderCommand(string OrderId, OrderStatus Status, int TotalQuantity) : IRequest<Order>;
}
