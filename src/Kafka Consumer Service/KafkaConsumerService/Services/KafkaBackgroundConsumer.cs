using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KafkaConsumerService.Models;
using System.Text.Json;

namespace KafkaConsumerService.Services
{
    public class KafkaBackgroundConsumer : BackgroundService
    {
        private readonly ILogger<KafkaBackgroundConsumer> _logger;
        private readonly KafkaSettings _settings;

        public KafkaBackgroundConsumer(IOptions<KafkaSettings> settings, ILogger<KafkaBackgroundConsumer> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_settings.Topic);

            _logger.LogInformation("Kafka consumer started. Topic: {Topic}", _settings.Topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);
                        if (result?.Message?.Value != null)
                        {
                            _logger.LogInformation("Message received at {TP}: {Value}", result.TopicPartitionOffset, result.Message.Value);
                            try
                            {
                                var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);
                                _logger.LogInformation("Parsed event: OrderId={OrderId}, CreatedAt={CreatedAt}", evt?.OrderId, evt?.CreatedAt);
                                // TODO: call NotificationService HTTP endpoint here if desired.
                            }
                            catch (JsonException jex)
                            {
                                _logger.LogWarning(jex, "Failed to deserialize message payload.");
                            }
                        }
                    }
                    catch (ConsumeException cex)
                    {
                        _logger.LogError(cex, "Consume error");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer stopping (cancelled).");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
