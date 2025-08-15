using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.BackgroundJobs;

public interface IKafkaProducer
{
    Task<KafkaProducerResult> ProduceAsync(OrderStatus orderStatus, string key, string value, CancellationToken cancellationToken = default);
}
