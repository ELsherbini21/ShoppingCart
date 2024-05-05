using Microsoft.EntityFrameworkCore.Query.Internal;
using ShoppingCart.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.BLL.Interfaces
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        void Add(T entity);

        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        void Update(T entity);

        void Delete(T entity);

    }
}
