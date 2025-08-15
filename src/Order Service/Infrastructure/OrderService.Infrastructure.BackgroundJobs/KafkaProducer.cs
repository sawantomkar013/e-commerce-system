using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.Helpers;

namespace OrderService.Infrastructure.BackgroundJobs;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;

    public KafkaProducer(IOptions<KafkaSettings> options)
    {
        _settings = options.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings?.BootstrapServers ?? "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<KafkaProducerResult> ProduceAsync(OrderStatus status, string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var topic = $"{_settings.BaseTopic}{OrderProcessingHelper.GetKafkaTopicSuffix(status)}";
            var result = await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value }, cancellationToken);

            Console.WriteLine($"Kafka published to {result.Topic} @ {result.Offset}");

            return new KafkaProducerResult
            {
                Success = true,
                Offset = result.TopicPartitionOffset
            };
        }
        catch (ProduceException<string, string> ex)
        {
            return new KafkaProducerResult
            {
                Success = false,
                ErrorReason = ex.Error.Reason
            };
        }
        catch (Exception ex)
        {
            return new KafkaProducerResult
            {
                Success = false,
                ErrorReason = ex.Message
            };
        }
    }

    public void Dispose() => _producer.Dispose();
}
