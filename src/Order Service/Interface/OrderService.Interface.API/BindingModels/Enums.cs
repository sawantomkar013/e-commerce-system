using System.Runtime.Serialization;

namespace OrderService.Interface.API.BindingModels;

public enum OrderStatus
{
    [EnumMember(Value = "pending")]
    Pending,
    [EnumMember(Value = "confirmed")]
    Confirmed,
    [EnumMember(Value = "shipped")]
    Shipped,
    [EnumMember(Value = "cancelled")]
    Cancelled
}
