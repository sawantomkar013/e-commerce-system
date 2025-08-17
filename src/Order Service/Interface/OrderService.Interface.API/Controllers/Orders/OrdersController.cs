using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Infrastructure.Caching;
using OrderService.Interface.API.BindingModels.Orders;
using OrderService.Interface.API.Commands.Orders;
using OrderService.Interface.API.Mappers.Orders;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Interface.API.Controllers.Orders;

[ApiController]
[Route("api/v1")]
public class OrdersController(
    IMediator mediator,
    IRedisCacheService cache,
    OrderMapper mapper) : ControllerBase
{
    [HttpPost("orders")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequestEntity createOrderRequestEntity,
        CancellationToken cancellationToken)
    {
        var orderRequest = new CreateOrderRequest(createOrderRequestEntity);
        var order = await mediator.Send(orderRequest, cancellationToken);

        // Cache newly created order
        var cacheKey = $"order:{order.Value.OrderId}";
        await cache.SetAsync(cacheKey, order.Value, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { uuid = order.Value.OrderId },
            mapper.OrderToOrderResponseEntity(order.Value));
    }

    [HttpGet("orders/{uuid:guid}")]
    public async Task<IActionResult> GetById(
        [Required] Guid uuid,
        CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(uuid), cancellationToken);
        if (order is null) return NotFound();

        return Ok(mapper.OrderToOrderResponseEntity(order));
    }
}
