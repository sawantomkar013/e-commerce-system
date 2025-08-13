using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrderService.Domain.DataAccess.Entities
{
    public class Order
    {
        [Key]
        [Required]
        public string OrderId { get; set; } = default!;

        public OrderStatus Status { get; set; }

        public int TotalQuantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string AuditTag => Status switch
        {
            OrderStatus.Pending => "PENDING_FLOW",
            OrderStatus.Confirmed => "CONFIRMED_FLOW",
            OrderStatus.Shipped => "SHIPPED_FLOW",
            OrderStatus.Cancelled => "CANCELLED_FLOW",
            _ => "UNKNOWN"
        };

        [NotMapped]
        [JsonIgnore]
        public TimeSpan NotificationDelay => Status switch
        {
            OrderStatus.Shipped => TimeSpan.FromSeconds(5),
            _ => TimeSpan.Zero
        };

        [NotMapped]
        [JsonIgnore]
        public string? KafkaTopicSuffix => Status switch
        {
            OrderStatus.Pending => ".pending",
            OrderStatus.Confirmed => ".confirmed",
            OrderStatus.Shipped => ".shipped",
            OrderStatus.Cancelled => null,
            _ => null
        };

        [NotMapped]
        [JsonIgnore]
        public int RetryAttempts => Status switch
        {
            OrderStatus.Pending => 3,
            OrderStatus.Confirmed => 5,
            OrderStatus.Shipped => 2,
            OrderStatus.Cancelled => 0,
            _ => 0
        };
    }
}
