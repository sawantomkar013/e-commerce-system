using Confluent.Kafka;

namespace OrderService.Infrastructure.BackgroundJobs;

public record KafkaProducerResult
{
    public bool Success { get; init; }

    public string? ErrorReason { get; init; }

    public TopicPartitionOffset? Offset { get; init; }
}