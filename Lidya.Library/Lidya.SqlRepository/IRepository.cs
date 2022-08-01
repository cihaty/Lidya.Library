using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lidya.SqlRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(object id);
        Task Insert(T entity);
        Task Insert(IEnumerable<T> entities);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        Task<T> FindOne(Expression<Func<T, bool>> predicate);
        IQueryable<T> List(Expression<Func<T, bool>> predicate);
        Task<bool> Exist(Expression<Func<T, bool>> predicate);
        IQueryable<T> Queryable { get; }
        DbSet<T> Entity { get; }
        Task SaveAsync();
        void Save();
    }
}
