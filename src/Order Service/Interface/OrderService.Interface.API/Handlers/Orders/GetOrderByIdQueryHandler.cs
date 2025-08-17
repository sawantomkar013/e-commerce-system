using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.Caching;
using OrderService.Interface.API.Commands.Orders;

namespace OrderService.Interface.API.Handlers.Orders;

public class GetOrderByIdQueryHandler(OrderDbContext db, IRedisCacheService cache) : IRequestHandler<GetOrderByIdQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{request.OrderId}";
        var cached = await cache.GetAsync<Order?>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId, cancellationToken);
        if (order is null) return null;

        await cache.SetAsync<Order?>(cacheKey, order, cancellationToken);

        return order;
    }
}

