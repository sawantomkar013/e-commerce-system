using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.DataAccess.Entities;

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
