namespace ShoppingCart.PL.Helpers.Enums
{
    public enum OrderStatusEnum
    {
        Pending, Refunded, Approved, Cancelled, UnderProcessing, Shipped
    }

    public enum PaymentStatusEnum
    {
        Pending, Approved, Rejected, Delay
    }
}
