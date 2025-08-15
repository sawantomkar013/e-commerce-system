using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.Commands.Orders;
using System.Text.Json;

namespace OrderService.Interface.API.Handlers.Orders;

public class GetOrderByIdQueryHandler(OrderDbContext db, IDistributedCache cache) : IRequestHandler<GetOrderByIdQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{request.OrderId}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Order>(cached);
        }

        var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId, cancellationToken);
        if (order is null) return null;

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(order), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        }, cancellationToken);

        return order;
    }
}

