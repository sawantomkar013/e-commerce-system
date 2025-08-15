using MediatR;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Interface.API.Commands.Orders;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Order?>;
