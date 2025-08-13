namespace OrderService.Infrastructure.ServiceClients
{
    public interface INotificationClient
    {
        Task SendAsync(object payload, int retryAttempts, CancellationToken ct);
    }
}
