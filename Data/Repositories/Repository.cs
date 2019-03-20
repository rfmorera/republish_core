using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Republish.Data;
using Republish.Data.RepositoriesInterfaces;

namespace Republish.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;

        public Repository(ApplicationDbContext _context)
        {
            this.Context = _context;
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        public T Get(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public T GetSingle()
        {
            return Context.Set<T>().Single();
        }

        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        /*Start using async methods*/

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T t)
        {
            Context.Set<T>().Add(t);
            await Context.SaveChangesAsync();
            return t;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await Context.Set<T>().SingleOrDefaultAsync(match);
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await Context.Set<T>().Where(match).ToListAsync();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            return await Context.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
                return null;
            T exist = null;
            if (key.GetType() == typeof(int[]))
            {
                exist = await Context.Set<T>().FindAsync(((int[])key)[0], ((int[])key)[1]);
            }
            else
            {
                exist = await Context.Set<T>().FindAsync(key);
            }
            if (exist != null)
            {
                Context.Entry(exist).CurrentValues.SetValues(t);
                await Context.SaveChangesAsync();
            }
            return exist;
        }

        public async Task<int> CountAsync()
        {
            return await Context.Set<T>().CountAsync();
        }

        public async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = Context.Set<T>();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<T, object>(includeProperty);
            }

            return queryable;
        }

        public IQueryable<T> QueryAll()
        {
            return Context.Set<T>();
        }
    }
}
