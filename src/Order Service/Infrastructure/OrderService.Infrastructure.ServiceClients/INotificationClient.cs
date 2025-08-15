using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.ServiceClients;

public interface INotificationClient
{
    Task<NotificationResult> SendAsync(OrderStatus orderStatus, object payload, CancellationToken cancellationToken);
}
