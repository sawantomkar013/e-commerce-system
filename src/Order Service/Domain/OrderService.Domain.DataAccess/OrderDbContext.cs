using Microsoft.EntityFrameworkCore;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Domain.DataAccess
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(b =>
            {
                b.ToTable("Orders");
                b.HasKey(x => x.OrderId);
                b.Property(x => x.Status).HasConversion<int>();
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.ToTable("AuditLogs");
                b.HasKey(x => x.Id);
                b.Property(x => x.CreatedAt);
            });
        }
    }
}
