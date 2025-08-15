using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.DataAccess;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands.Orders;
using OrderService.Interface.API.Mappers.Orders;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OrderService.Interface.API.Controllers.Orders;

[ApiController]
[Route("api/v1")]
public class OrdersController(
    IMediator mediator, 
    IDistributedCache cache, 
    OrderDbContext db,
    OrderMapper mapper) : ControllerBase
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
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(order.Value), cacheOptions, cancellationToken);

        return CreatedAtAction(
            nameof(GetById), 
            new { uuid = order.Value.OrderId }, 
            mapper.OrderToOrderResponseEntity(order.Value));
    }

    [HttpGet("orders/{uuid:guid}")]
    public async Task<IActionResult> GetById(
        [Required]Guid uuid, 
        CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(uuid), cancellationToken);
        if (order is null) return NotFound();

        return Ok(mapper.OrderToOrderResponseEntity(order));
    }
}
