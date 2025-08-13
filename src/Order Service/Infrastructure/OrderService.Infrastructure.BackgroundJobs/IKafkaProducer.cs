namespace OrderService.Infrastructure.BackgroundJobs
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string value, CancellationToken ct = default);
    }
}
