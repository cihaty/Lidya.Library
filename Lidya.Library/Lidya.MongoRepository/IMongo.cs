using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Lidya.MongoRepository
{
    public interface IMongo<T> : IQueryable<T>, IDisposable
        where T : class
    {
        IMongoCollection<T> Collection { get; }

        IMongoCollection<T> GetCollection(string collectionName);

        IFindFluent<T, T> Find();
        IFindFluent<T, T> Find(FilterDefinition<T> filter, FindOptions options = null);
        IFindFluent<T, T> Find(Expression<Func<T, bool>> filter, FindOptions options = null);

        T GetById(object id, string collectionName = "");
        T GetById(Expression<Func<T, bool>> predicate, string collectionName = "");
        Task<T> GetByIdAsync(object id, string collectionName = "");
        Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate, string collectionName = "");

        T Add(T entity, string collectionName = "");
        BulkWriteResult<T> BulkWrite(IEnumerable<WriteModel<T>> requests, string collectionName = "", BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> AddAsync(T entity, string collectionName = "");
        void Add(IEnumerable<T> entities, string collectionName = "");
        Task AddAsync(IEnumerable<T> entities, string collectionName = "");

        bool Update(T entity, string collectionName = "");
        bool Update(List<T> entity, string collectionName = "");
        bool AddOrUpdate(T entity, string collectionName = "");
        Task<bool> UpdateAsync(T entity, string collectionName = "");
        Task<bool> UpdateAsync(List<T> entity, string collectionName = "");
        UpdateResult UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        UpdateResult UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<UpdateResult> UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        UpdateResult UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<UpdateResult> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> AddOrUpdateAsync(T entity, string collectionName = "", CancellationToken cancellationToken = default(CancellationToken));
        UpdateResult UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<UpdateResult> UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

        IMongoQueryable<T> List(string collectionName = "");
        IQueryable<T> List(Expression<Func<T, bool>> predicate, string collectionName = "");

        void Delete(object id);
        Task DeleteAsync(object id);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);
        void DeleteAll();
        Task DeleteAllAsync();

        long Count();
        Task<long> CountAsync();
        long Count(FilterDefinition<T> filter);
        Task<long> CountAsync(FilterDefinition<T> filter);
        long Count(Expression<Func<T, bool>> predicate);
        Task<long> CountAsync(Expression<Func<T, bool>> predicate);
        bool Exists(Expression<Func<T, bool>> predicate);
    }

    public interface IMongo : IDisposable
    {
        IMongoCollection<T> Collection<T>(string collectionName) where T : class;
        IMongoCollection<BsonDocument> Collection(string collectionName);

        T GetById<T>(object id, string collectionName = "") where T : class;
        Task<T> GetByIdAsync<T>(object id, string collectionName = "") where T : class;

        T Add<T>(T entity, string collectionName = "") where T : class;
        Task<T> AddAsync<T>(T entity, string collectionName = "") where T : class;
        void Add<T>(IEnumerable<T> entities, string collectionName = "") where T : class;
        Task AddAsync<T>(IEnumerable<T> entities, string collectionName = "") where T : class;

        bool Update<T>(T entity, string collectionName = "") where T : class;
        bool Update<T>(List<T> entity, string collectionName = "") where T : class;
        bool AddOrUpdate<T>(T entity, string collectionName = "") where T : class;
        
        Task<bool> UpdateAsync<T>(T entity, string collectionName = "") where T : class;
        Task<bool> UpdateAsync<T>(List<T> entity, string collectionName = "") where T : class;
        Task<bool> AddOrUpdateAsync<T>(T entity, string collectionName = "") where T : class;

        IQueryable<T> List<T>(string collectionName = "") where T : class;
        IQueryable<T> List<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class;

        void Delete<T>(object id, string collectionName = "") where T : class;
        Task DeleteAsync<T>(object id, string collectionName = "") where T : class;
        void Delete<T>(T entity, string collectionName = "") where T : class;
        Task DeleteAsync<T>(T entity, string collectionName = "") where T : class;
        void Delete<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class;
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class;
        void DeleteAll<T>(string collectionName = "") where T : class;
        Task DeleteAllAsync<T>(string collectionName = "") where T : class;

        long Count<T>(string collectionName = "") where T : class;

        bool Exists<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class;
    }
}
