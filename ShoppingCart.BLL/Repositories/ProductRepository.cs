using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;

namespace ShoppingCart.BLL.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
