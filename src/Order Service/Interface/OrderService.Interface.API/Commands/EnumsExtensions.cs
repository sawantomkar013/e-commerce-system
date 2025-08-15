using System.ComponentModel;

namespace OrderService.Interface.API.Commands;

public static class EnumsExtensions
{
    public static Domain.DataAccess.Entities.OrderStatus GetDomainOrderStatus(this BindingModels.OrderStatus orderStatus)
    {
        switch (orderStatus)
        {
            case BindingModels.OrderStatus.Pending:
                return Domain.DataAccess.Entities.OrderStatus.Pending;
            case BindingModels.OrderStatus.Confirmed:
                return Domain.DataAccess.Entities.OrderStatus.Confirmed;
            case BindingModels.OrderStatus.Shipped:
                return Domain.DataAccess.Entities.OrderStatus.Shipped;
            case BindingModels.OrderStatus.Cancelled:
                return Domain.DataAccess.Entities.OrderStatus.Cancelled;
            default:
                throw new InvalidEnumArgumentException(nameof(orderStatus));
        }
    }
}
