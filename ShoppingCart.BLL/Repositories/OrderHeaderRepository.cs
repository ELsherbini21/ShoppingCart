using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;
using System.Data;

namespace ShoppingCart.BLL.Repositories
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }


        public void UpdateStatus(int id, string orderStatus, string? paymentStatus)
        {
            var orderHeader = _appDbContext.OrderHeaders.FirstOrDefault(orderHeader => orderHeader.Id == id);

            if (orderHeader is not null)
                orderHeader.OrderStatus = orderStatus;

            if (paymentStatus is not null)
                orderHeader.PaymentStatus = paymentStatus;
        }
        public void PaymentStatus(int id, string sessionId, string paymentIntentId, DateTime? dateOfPayment)
        {
            var orderHeader = _appDbContext.OrderHeaders.FirstOrDefault(orderHeader => orderHeader.Id == id);

            if (orderHeader is not null)
            {
                orderHeader.DateOfPayment = dateOfPayment ?? DateTime.Now;

                orderHeader.SessionId = sessionId;

                orderHeader.PaymentIntentId = paymentIntentId;
            }
        }
    }
}
