using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Republish.Data.RepositoriesInterfaces
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        IEnumerable<T> GetAll();

        T GetSingle();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        /*Start using async methods*/

        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

        Task<T> AddAsync(T t);

        Task<IEnumerable<T>> AddAllAsync(IEnumerable<T> tList);

        Task<int> CountAsync();

        Task<int> DeleteAsync(T entity);

        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);

        Task<T> FindAsync(Expression<Func<T, bool>> match);

        Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        Task<ICollection<T>> GetAllAsync();

        Task<T> GetAsync(int id);

        Task<T> UpdateAsync(T t, object key);
        T Update(T t, object key);

        IQueryable<T> QueryAll();
    }
}