using OrderService.Domain.DataAccess;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.EntityFramework
{
    public static class AuditingRules
    {
        public static async Task WriteAuditAsync(OrderDbContext db, string orderId, string tag, string message, CancellationToken ct)
        {
            db.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Tag = tag,
                Message = message,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
