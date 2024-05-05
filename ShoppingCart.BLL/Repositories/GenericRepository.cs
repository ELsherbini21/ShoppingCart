using Microsoft.EntityFrameworkCore;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ModelBase
    {
        private protected readonly AppDbContext _appDbContext;

        public GenericRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async void Add(T entity) => await _appDbContext.AddAsync(entity);

        public async Task<T> GetByIdAsync(int id) => await _appDbContext.FindAsync<T>(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _appDbContext.Set<T>().ToListAsync();

        public void Update(T entity) => _appDbContext.Set<T>().Update(entity);

        public void Delete(T entity) => _appDbContext.Remove(entity);

    }
}
