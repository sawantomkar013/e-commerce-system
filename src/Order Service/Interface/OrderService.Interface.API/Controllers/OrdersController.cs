using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.DataAccess;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands.Orders;
using System.Text.Json;

namespace OrderService.Interface.API.Controllers;

[ApiController]
[Route("api/v1")]
public class OrdersController(IMediator mediator, IDistributedCache cache, OrderDbContext db) : ControllerBase
{
    [HttpPost("orders")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequestEntity createOrderRequestEntity, 
        CancellationToken cancellationToken)
    {
        var orderRequest = new CreateOrderRequest(createOrderRequestEntity);

        var order = await mediator.Send(orderRequest, cancellationToken);

        var cacheKey = $"order:{order.Value.OrderId}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(order), cacheOptions, cancellationToken);

        return Ok(order); // CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    [HttpGet("orders/{uuid:guid}")]
    public async Task<IActionResult> GetById(Guid uuid, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{uuid}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            var order = JsonSerializer.Deserialize<Domain.DataAccess.Entities.Order>(cached!);
            return Ok(order);
        }

        var fromDb = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == uuid, cancellationToken);
        if (fromDb is null) return NotFound();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(fromDb), cacheOptions, cancellationToken);
        return Ok(fromDb);
    }
}
