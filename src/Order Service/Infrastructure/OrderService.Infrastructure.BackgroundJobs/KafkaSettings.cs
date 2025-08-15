namespace OrderService.Infrastructure.BackgroundJobs;

public record KafkaSettings
{
    public string BootstrapServers { get; init; } = "localhost:9092";

    public string BaseTopic { get; init; } = "orders.created";
}
