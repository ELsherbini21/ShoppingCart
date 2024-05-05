using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;

namespace ShoppingCart.BLL.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
