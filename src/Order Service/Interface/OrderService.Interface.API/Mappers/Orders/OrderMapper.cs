using OrderService.Domain.DataAccess.Entities;
using OrderService.Interface.API.BindingModels.Orders;
using Riok.Mapperly.Abstractions;

namespace OrderService.Interface.API.Mappers.Orders;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    EnumMappingStrategy = EnumMappingStrategy.ByName,
    EnumNamingStrategy = EnumNamingStrategy.SerializationEnumMemberAttribute)]
public partial class OrderMapper
{
    [MapProperty(nameof(Order.OrderId), nameof(OrderResponseEntity.OrderUuid))]
    [MapProperty(nameof(Order.CustomerId), nameof(OrderResponseEntity.CustomerUuid))]
    [MapProperty(nameof(Order.ProductId), nameof(OrderResponseEntity.ProductUuid))]
    [MapProperty(nameof(Order.TotalQuantity), nameof(OrderResponseEntity.TotalQuantity))]
    [MapProperty(nameof(Order.Status), nameof(OrderResponseEntity.Status))]
    [MapProperty(nameof(Order.CreatedAt), nameof(OrderResponseEntity.CreatedAt))]
    public partial OrderResponseEntity OrderToOrderResponseEntity(Order order);
}
