namespace OrderService.Domain.DataAccess.Entities
{
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Shipped = 2,
        Cancelled = 3
    }
}
