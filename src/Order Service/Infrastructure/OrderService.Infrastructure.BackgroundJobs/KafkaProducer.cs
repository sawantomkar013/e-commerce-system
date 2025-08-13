using Confluent.Kafka;

namespace OrderService.Infrastructure.BackgroundJobs
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        // private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(ProducerConfig config) // , ILogger<KafkaProducer> logger)
        {
            _producer = new ProducerBuilder<string, string>(config).Build();
            // _logger = logger;
        }

        public async Task ProduceAsync(string topic, string key, string value, CancellationToken ct = default)
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value }, ct);
            // _logger.LogInformation("Kafka published to {Topic} @ {Offset}", result.Topic, result.Offset);
        }

        public void Dispose() => _producer.Dispose();
    }
}
