using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.EntityFramework;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context == null) return;

        var changedOrders = context.ChangeTracker.Entries<Order>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in changedOrders)
        {
            var order = entry.Entity;

            var tag = order.Status switch
            {
                OrderStatus.Pending => "PENDING_FLOW",
                OrderStatus.Confirmed => "CONFIRMED_FLOW",
                OrderStatus.Shipped => "SHIPPED_FLOW",
                OrderStatus.Cancelled => "CANCELLED_FLOW",
                _ => "UNKNOWN"
            };

            var message = entry.State switch
            {
                EntityState.Added => $"Order {order.OrderId} created with status {order.Status}.",
                EntityState.Modified => $"Order {order.OrderId} updated. Status: {order.Status}.",
                _ => $"Order {order.OrderId} changed."
            };

            context.Set<AuditLog>().Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                OrderId = order.OrderId.ToString(),
                Tag = tag,
                Message = message,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
    }
}
