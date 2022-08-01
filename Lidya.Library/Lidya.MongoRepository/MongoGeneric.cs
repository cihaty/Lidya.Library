using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Lidya.MongoRepository
{
    public class Mongo<T> : IMongo<T>, IDisposable
        where T : class
    {
        public IMongoDatabase Database;
        readonly string _collectionName;
        public Mongo(MongoDbContext mongoDb)
        {
            Database = mongoDb.Database;
            _collectionName = typeof(T).Name;
        }

        public Mongo(MongoDbContext mongoDb, string collectionName)
        {
            Database = mongoDb.Database;
            _collectionName = collectionName;
        }

        IMongoCollection<T> _collection;

        public IMongoCollection<T> Collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = Database.GetCollection<T>(_collectionName);
                }
                return this._collection;
            }
        }

        public IMongoCollection<T> GetCollection(string collectionName = "")
        {
            if (!string.IsNullOrEmpty(collectionName))
            {
                return Database.GetCollection<T>(collectionName);
            }
            else
            {
                return Collection;
            }
        }

        public IFindFluent<T, T> Find()
        {
            return Collection.Find(Builders<T>.Filter.Empty);
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> filter, FindOptions options = null)
        {
            return Collection.Find(filter, options);
        }

        public IFindFluent<T, T> Find(Expression<Func<T, bool>> filter, FindOptions options = null)
        {
            return Collection.Find(filter, options);
        }

        public virtual T GetById(object id, string collectionName = "")
        {
            return GetCollection(collectionName).Find(GetByIdPredicate(id)).FirstOrDefault();
        }

        public virtual T GetById(Expression<Func<T, bool>> predicate, string collectionName = "")
        {
            return GetCollection(collectionName).Find(predicate).FirstOrDefault();
        }
        public virtual async Task<T> GetByIdAsync(object id, string collectionName = "")
        {
            return await GetCollection(collectionName).Find(GetByIdPredicate(id)).FirstOrDefaultAsync();
        }
        public virtual async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate, string collectionName = "")
        {
            return await GetCollection(collectionName).Find(predicate).FirstOrDefaultAsync();
        }

        public T Add(T entity, string collectionName = "")
        {
            Collection.InsertOne(entity);
            return entity;
        }

        public BulkWriteResult<T> BulkWrite(IEnumerable<WriteModel<T>> requests, string collectionName = "", BulkWriteOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCollection(collectionName).BulkWrite(requests, options, cancellationToken);
        }

        public async Task<T> AddAsync(T entity, string collectionName = "")
        {
            await Collection.InsertOneAsync(entity);
            return entity;
        }
        public void Add(IEnumerable<T> entities, string collectionName = "")
        {
            Collection.InsertMany(entities);
        }
        public async Task AddAsync(IEnumerable<T> entities, string collectionName = "")
        {
            await Collection.InsertManyAsync(entities);
        }

        #region Update

        public virtual bool Update(T entity, string collectionName = "")
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            ReplaceOneResult result = Collection.ReplaceOne(GetByIdPredicate(val), entity);
            return result.ModifiedCount > 0;
        }

        public virtual bool AddOrUpdate(T entity, string collectionName = "")
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            var option = new UpdateOptions();
            option.IsUpsert = true;
            ReplaceOneResult result = Collection.ReplaceOne(GetByIdPredicate(val), entity, option);
            return (result.ModifiedCount > 0 || result.UpsertedId > 0) || (result.IsAcknowledged && result.IsModifiedCountAvailable && result.MatchedCount == 1);
        }

        public virtual bool Update(List<T> entity, string collectionName = "")
        {
            var models = new WriteModel<T>[entity.Count];
            for (var i = 0; i < entity.Count; i++)
            {
                var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
                models[i] = new ReplaceOneModel<T>(Builders<T>.Filter.Eq("_id", val), entity[i]);
            }
            GetCollection(collectionName).BulkWrite(models);
            return true;
        }
        public virtual async Task<bool> UpdateAsync(T entity, string collectionName = "")
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            ReplaceOneResult result = await Collection.ReplaceOneAsync(GetByIdPredicate(val), entity);
            return result.ModifiedCount > 0;
        }

        public virtual async Task<bool> AddOrUpdateAsync(T entity, string collectionName = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            var option = new UpdateOptions();
            option.IsUpsert = true;
            ReplaceOneResult result = await Collection.ReplaceOneAsync(GetByIdPredicate(val), entity, option, cancellationToken);
            return result.ModifiedCount > 0 || result.UpsertedId > 0;
        }

        public virtual async Task<bool> UpdateAsync(List<T> entity, string collectionName = "")
        {
            var models = new WriteModel<T>[entity.Count];
            for (var i = 0; i < entity.Count; i++)
            {
                var val = GetBsonIdProp(entity[i].GetType()).GetValue(entity[i], null);
                models[i] = new ReplaceOneModel<T>(Builders<T>.Filter.Eq("_id", val), entity[i]);
            }
            await GetCollection(collectionName).BulkWriteAsync(models);
            return true;
        }

        public virtual UpdateResult UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateMany(filter, update, options, cancellationToken);
        }

        public virtual Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateManyAsync(filter, update, options, cancellationToken);
        }

        public virtual UpdateResult UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateMany(filter, update, options, cancellationToken);
        }

        public virtual Task<UpdateResult> UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateManyAsync(filter, update, options, cancellationToken);
        }

        public virtual UpdateResult UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateOne(filter, update, options, cancellationToken);
        }

        public virtual Task<UpdateResult> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        public virtual UpdateResult UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateOne(filter, update, options, cancellationToken);
        }

        public virtual Task<UpdateResult> UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }


        #endregion

        #region List

        public IMongoQueryable<T> List(string collectionName = "")
        {
            return GetCollection(collectionName).AsQueryable();
        }

        public IQueryable<T> List(Expression<Func<T, bool>> predicate, string collectionName = "")
        {
            return List(collectionName).Where(predicate);
        }

        #endregion

        #region Delete

        public void Delete(object id)
        {
            Collection.DeleteOne(Builders<T>.Filter.Eq("_id", id));
        }

        public async Task DeleteAsync(object id)
        {
            await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }

        public void Delete(T entity)
        {
            Delete(GetBsonIdProp(entity.GetType()).GetValue(entity, null));
        }

        public async Task DeleteAsync(T entity)
        {
            await DeleteAsync(GetBsonIdProp(entity.GetType()).GetValue(entity, null));
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            Collection.DeleteMany(predicate);
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await Collection.DeleteManyAsync(predicate);
        }

        public void DeleteAll()
        {
            Collection.DeleteMany(new BsonDocument());
        }

        public async Task DeleteAllAsync()
        {
            await Collection.DeleteManyAsync(new BsonDocument());
        }

        #endregion


        public long Count(FilterDefinition<T> filter)
        {
            ProjectionDefinition<T, BsonDocument> projectFilter = "{ x: 1 }";
            return Collection.Find(filter).Project(projectFilter).Count();
        }

        public Task<long> CountAsync(FilterDefinition<T> filter)
        {
            ProjectionDefinition<T, BsonDocument> projectFilter = "{ x: 1 }";
            return Collection.Find(filter).Project(projectFilter).CountAsync();
        }

        public long Count(Expression<Func<T, bool>> predicate)
        {
            ProjectionDefinition<T, BsonDocument> filter = "{ x: 1 }";
            return Collection.Find(predicate).Project(filter).Count();
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            ProjectionDefinition<T, BsonDocument> filter = "{ x: 1 }";
            return Collection.Find(predicate).Project(filter).CountAsync();
        }

        public long Count()
        {
            ProjectionDefinition<T,BsonDocument> filter = "{ x: 1 }";
            return Collection.Find(Builders<T>.Filter.Empty).Project(filter).Count();
        }

        public Task<long> CountAsync()
        {
            ProjectionDefinition<T, BsonDocument> filter = "{ x: 1 }";
            return Collection.Find(Builders<T>.Filter.Empty).Project(filter).CountAsync();
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return Collection.AsQueryable().Any(predicate);
        }

        #region IQueryable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.AsQueryable<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.AsQueryable<T>().GetEnumerator();
        }

        public Type ElementType
        {
            get { return Collection.AsQueryable<T>().ElementType; }
        }

        public Expression Expression
        {
            get { return Collection.AsQueryable<T>().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return Collection.AsQueryable<T>().Provider; }
        }

        #endregion

        private Expression<Func<T, bool>> GetByIdPredicate(object value)
        {
            var type = typeof(T);
            var p = GetBsonIdProp(type);
            var parameter = Expression.Parameter(type, "x");
            var member = Expression.Property(parameter, p.Name);
            var constant = Expression.Constant(value);
            var body = Expression.Equal(member, constant);
            var finalExpression = Expression.Lambda<Func<T, bool>>(body, new ParameterExpression[] { parameter });
            return finalExpression;
        }

        private PropertyInfo GetBsonIdProp(Type type)
        {
            return type.GetProperties().First(x => x.GetCustomAttributes(typeof(BsonIdAttribute), false).Length > 0);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
