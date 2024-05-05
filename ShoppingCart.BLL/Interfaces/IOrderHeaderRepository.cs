using ShoppingCart.DAL.Models;

namespace ShoppingCart.BLL.Interfaces
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void UpdateStatus(int id , string orderStatus , string ?paymentStatus);
        void PaymentStatus(int id , string sessionId, string paymentIntentId  ,DateTime? dateOfPayment);

    }
}
