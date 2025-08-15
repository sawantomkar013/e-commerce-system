namespace OrderService.Infrastructure.ServiceClients;

public record NotificationResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public int StatusCode { get; init; }
}

