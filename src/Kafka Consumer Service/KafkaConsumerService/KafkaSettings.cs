namespace KafkaConsumerService
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string GroupId { get; set; } = "order-consumers";
        public string Topic { get; set; } = "orders.created";
    }
}
