using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.BackgroundJobs;
using OrderService.Infrastructure.EntityFramework;
using OrderService.Infrastructure.ServiceClients;

namespace OrderService.Interface.API.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly OrderDbContext _db;
        private readonly IKafkaProducer _kafka;
        private readonly INotificationClient _notify;
        private readonly ILogger<CreateOrderHandler> _logger;
        private readonly string _kafkaBaseTopic;

        public CreateOrderHandler(
            OrderDbContext db,
            IKafkaProducer kafka,
            INotificationClient notify,
            ILogger<CreateOrderHandler> logger,
            IConfiguration config)
        {
            _db = db;
            _kafka = kafka;
            _notify = notify;
            _logger = logger;
            _kafkaBaseTopic = config.GetValue<string>("Kafka:BaseTopic") ?? "orders.created";
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            var order = new Order
            {
                OrderId = request.OrderId,
                Status = request.Status,
                TotalQuantity = request.TotalQuantity,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Save order
            var existing = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == order.OrderId, ct);
            if (existing is null) _db.Orders.Add(order); else _db.Orders.Update(order);
            await _db.SaveChangesAsync(ct);

            using (_logger.BeginScope(new Dictionary<string, object> { ["auditTag"] = order.AuditTag, ["orderId"] = order.OrderId }))
            {
                await AuditingRules.WriteAuditAsync(_db, order.OrderId, order.AuditTag, $"Processing {order.Status}", ct);

                if (order.Status == OrderStatus.Confirmed && order.TotalQuantity > 10)
                {
                    _logger.LogWarning("Potential fraud detected: High-volume confirmed order.");
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    _logger.LogInformation("Cancelled order: skipping notification and Kafka publish.");
                    return order;
                }

                if (order.NotificationDelay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Delaying notification by {Delay}", order.NotificationDelay);
                    await Task.Delay(order.NotificationDelay, ct);
                }

                var suffix = order.KafkaTopicSuffix ?? string.Empty;
                var topic = _kafkaBaseTopic + suffix;
                var payload = JsonSerializer.Serialize(new { orderId = order.OrderId, timestamp = order.CreatedAt });
                // await _kafka.ProduceAsync(topic, order.OrderId, payload, ct);

                var notifyPayload = new
                {
                    orderId = order.OrderId,
                    status = order.Status.ToString(),
                    totalQuantity = order.TotalQuantity,
                    createdAt = order.CreatedAt
                };
                await _notify.SendAsync(notifyPayload, order.RetryAttempts, ct);

                _logger.LogInformation("Order {OrderId} processed.", order.OrderId);
                return order;
            }
        }
    }
}
