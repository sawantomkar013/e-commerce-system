namespace OrderService.Infrastructure.Caching;

public record RedisSettings
{
    public string Connection { get; init; } = string.Empty;

    public int AbsoluteExpirationMinutes { get; init; }
}
