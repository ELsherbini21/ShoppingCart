using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; set; }

        IProductRepository ProductRepository { get; set; }

        ICartRepository CartRepository { get; set; }

        IOrderDetailRepository OrderDetailRepository { get; set; }

        IOrderHeaderRepository OrderHeaderRepository { get; set; }

        void BeginTransactionAsync();

        void CommitTransactionAsync();

        void RollbackTransactionAsync();

        Task Save();
    }
}
