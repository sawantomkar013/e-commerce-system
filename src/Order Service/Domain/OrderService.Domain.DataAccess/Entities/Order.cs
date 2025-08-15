using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrderService.Domain.DataAccess.Entities
{
    public class Order
    {
        [Key]
        [Required]
        public Guid OrderId { get; set; } = default!;

        public Guid CustomerId { get; set; }

        public Guid ProductId { get; set; }

        public int TotalQuantity { get; set; }

        public OrderStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
