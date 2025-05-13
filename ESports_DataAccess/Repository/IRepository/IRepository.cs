using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
