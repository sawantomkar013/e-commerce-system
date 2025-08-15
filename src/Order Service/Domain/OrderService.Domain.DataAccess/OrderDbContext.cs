using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Domain.DataAccess;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var type = property.ClrType;

                if (type.IsEnum)
                {
                    var converterType = typeof(EnumMemberValueConverter<>).MakeGenericType(type);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                    property.SetValueConverter(converter);
                }
                else if (Nullable.GetUnderlyingType(type)?.IsEnum == true)
                {
                    var enumType = Nullable.GetUnderlyingType(type)!;
                    var converterType = typeof(EnumMemberValueConverter<>).MakeGenericType(enumType);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                    property.SetValueConverter(converter);
                }
            }
        }

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
