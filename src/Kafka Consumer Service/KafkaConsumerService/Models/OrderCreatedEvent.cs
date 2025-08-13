namespace KafkaConsumerService.Models
{
    public class OrderCreatedEvent
    {
        public string? OrderId { get; set; }
        public System.DateTimeOffset CreatedAt { get; set; }
    }
}
