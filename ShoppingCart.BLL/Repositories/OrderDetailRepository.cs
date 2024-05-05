using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;

namespace ShoppingCart.BLL.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
