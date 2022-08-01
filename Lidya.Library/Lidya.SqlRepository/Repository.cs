using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lidya.SqlRepository
{
    public class Repository<T> : IRepository<T> where T : class

    {
        private readonly DbContext _context;
        private readonly DbSet<T> _entity;
        public Repository(DbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public virtual Task<T> GetById(object id) => _entity.FindAsync(id).AsTask();

        public async Task Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _context.Entry(entity);

            await _entity.AddAsync(entity);
        }

        public virtual async Task Insert(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                await _entity.AddAsync(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _entity.Update(entity);
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            _entity.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity.Remove(entity);
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                _entity.Remove(entity);
        }

        public virtual Task<T> FindOne(Expression<Func<T, bool>> predicate) => _entity.FirstOrDefaultAsync(predicate);

        public virtual IQueryable<T> List(Expression<Func<T, bool>> predicate) => _entity.Where(predicate);

        public virtual Task<bool> Exist(Expression<Func<T, bool>> predicate) => _entity.AnyAsync(predicate);

        public virtual IQueryable<T> Queryable => _entity.AsQueryable();
        public virtual DbSet<T> Entity => _entity;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
