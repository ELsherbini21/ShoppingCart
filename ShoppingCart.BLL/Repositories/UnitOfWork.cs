using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;


        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

            CategoryRepository = new CategoryRepository(appDbContext);
            ProductRepository = new ProductRepository(appDbContext);
            CartRepository = new CartRepository(appDbContext);
            OrderHeaderRepository = new OrderHeaderRepository(appDbContext);
            OrderDetailRepository = new OrderDetailRepository(appDbContext);

        }


        public ICategoryRepository CategoryRepository { get; set; }

        public IProductRepository ProductRepository { get; set; }

        public ICartRepository CartRepository { get; set; }

        public IOrderHeaderRepository OrderHeaderRepository { get; set; }

        public IOrderDetailRepository OrderDetailRepository { get; set; }


        public async Task Save() => await _appDbContext.SaveChangesAsync();

        public async void Dispose() => await _appDbContext.DisposeAsync();

        public async void BeginTransactionAsync() => await _appDbContext.Database.BeginTransactionAsync();

        public async void CommitTransactionAsync() => await _appDbContext.Database.CommitTransactionAsync();

        public async void RollbackTransactionAsync() => await _appDbContext.Database.RollbackTransactionAsync();
    }
}
