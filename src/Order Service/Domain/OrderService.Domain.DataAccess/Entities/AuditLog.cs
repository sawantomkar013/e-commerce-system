using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.DataAccess.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public string OrderId { get; set; } = default!;
        public string Tag { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
