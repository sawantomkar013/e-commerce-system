using MediatR;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using OrderService.Interface.API.Handlers;
using OrderService.Domain.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.DataAccess;

namespace OrderService.Interface.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDatabase _redis;
        private readonly OrderDbContext _db;

        public OrdersController(IMediator mediator, IConnectionMultiplexer mux, OrderDbContext db)
        {
            _mediator = mediator;
            _redis = mux.GetDatabase();
            _db = db;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.OrderId)) return BadRequest("OrderId required");
            if (!Enum.IsDefined(typeof(OrderStatus), cmd.Status)) return BadRequest("Invalid status");

            var order = await _mediator.Send(cmd, ct);

            var cacheKey = $"order:{order.OrderId}";
            await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(order), TimeSpan.FromMinutes(5));

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
        }

        [HttpGet("orders/{uuid:guid}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var cacheKey = $"order:{id}";
            var cached = await _redis.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                var order = JsonSerializer.Deserialize<Domain.DataAccess.Entities.Order>(cached!);
                return Ok(order);
            }

            var fromDb = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == id, ct);
            if (fromDb is null) return NotFound();

            await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(fromDb), TimeSpan.FromMinutes(5));
            return Ok(fromDb);
        }
    }
}
