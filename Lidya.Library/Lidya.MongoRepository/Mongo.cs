using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Lidya.MongoRepository
{
    public class Mongo : IMongo
    {

        MongoClient client;
        public IMongoDatabase Database;

        #region Constructor

        public Mongo(string connectionString, string dbName)
        {
            this.client = new MongoClient(new MongoUrl(connectionString));
            this.Database = this.client.GetDatabase(dbName);
        }

        public Mongo(IMongoDatabase mongoDatabase)
        {
            this.Database = mongoDatabase;
        }

        public Mongo(MongoUrl MongoUrl)
        {
            this.client = new MongoClient(MongoUrl);
            this.Database = this.client.GetDatabase(MongoUrl.DatabaseName);
        }

        #endregion

        public IMongoCollection<T> Collection<T>(string collectionName = "") where T : class
        {
            return Database.GetCollection<T>((!string.IsNullOrEmpty(collectionName) ? collectionName : typeof(T).Name));
        }

        public IMongoCollection<BsonDocument> Collection(string collectionName)
        {
            return Database.GetCollection<BsonDocument>(collectionName);
        }

        public T GetById<T>(object id, string collectionName = "") where T : class
        {
            //var val = GetBsonIdProp(entity[i].GetType()).GetValue(entity[i], null);
            return Collection<T>(collectionName).Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefault();
        }

        public T GetById<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            return Collection<T>(collectionName).Find(predicate).FirstOrDefault();
        }

        public async Task<T> GetByIdAsync<T>(object id, string collectionName = "") where T : class
        {
            return await Collection<T>(collectionName).Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        }

        public async Task<T> GetByIdAsync<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            return await Collection<T>(collectionName).Find(predicate).FirstOrDefaultAsync();
        }

        #region Create

        public T Add<T>(T entity, string collectionName = "") where T : class
        {
            Collection<T>(collectionName).InsertOne(entity);
            return entity;
        }

        public async Task<T> AddAsync<T>(T entity, string collectionName = "") where T : class
        {
            await Collection<T>(collectionName).InsertOneAsync(entity);
            return entity;
        }

        public void Add<T>(IEnumerable<T> entities, string collectionName = "") where T : class
        {
            Collection<T>(collectionName).InsertMany(entities);
        }

        public async Task AddAsync<T>(IEnumerable<T> entities, string collectionName = "") where T : class
        {
            await Collection<T>(collectionName).InsertManyAsync(entities);
        }

        #endregion

        #region Update
        public bool Update<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            ReplaceOneResult result = Collection<T>(collectionName).ReplaceOne(Builders<T>.Filter.Eq("_id", val), entity);
            return result.ModifiedCount > 0;
        }
        public bool Update<T>(List<T> entity, string collectionName = "") where T : class
        {
            var models = new WriteModel<T>[entity.Count];
            for (var i = 0; i < entity.Count; i++)
            {
                var val = GetBsonIdProp(entity[i].GetType()).GetValue(entity[i], null);
                models[i] = new ReplaceOneModel<T>(Builders<T>.Filter.Eq("_id", val), entity[i]);
            }
            Collection<T>(collectionName).BulkWrite(models);
            return true;
        }
        public bool AddOrUpdate<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            var option = new UpdateOptions();
            option.IsUpsert = true;
            ReplaceOneResult result = Collection<T>(collectionName).ReplaceOne(Builders<T>.Filter.Eq("_id", val), entity, option);
            return result.ModifiedCount > 0 || result.UpsertedId > 0;
        }
        public async Task<bool> UpdateAsync<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            ReplaceOneResult result = await Collection<T>(collectionName).ReplaceOneAsync(Builders<T>.Filter.Eq("_id", val), entity);
            return result.ModifiedCount > 0;
        }
        public async Task<bool> UpdateAsync<T>(List<T> entity, string collectionName = "") where T : class
        {
            var models = new WriteModel<T>[entity.Count];
            for (var i = 0; i < entity.Count; i++)
            {
                var val = GetBsonIdProp(entity[i].GetType()).GetValue(entity[i], null);
                models[i] = new ReplaceOneModel<T>(Builders<T>.Filter.Eq("_id", val), entity[i]);
            }
            await Collection<T>(collectionName).BulkWriteAsync(models);
            return true;
        }
        public async Task<bool> AddOrUpdateAsync<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            var option = new UpdateOptions();
            option.IsUpsert = true;
            ReplaceOneResult result = await Collection<T>(collectionName).ReplaceOneAsync(Builders<T>.Filter.Eq("_id", val), entity, option);
            return result.ModifiedCount > 0 || result.UpsertedId > 0;
        }

        #endregion

        #region List

        public IQueryable<T> List<T>(string collectionName = "") where T : class
        {
            return Collection<T>(collectionName).AsQueryable();
        }

        public IQueryable<T> List<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            return List<T>(collectionName).Where(predicate);
        }

        #endregion

        #region Delete
        public void Delete<T>(object id, string collectionName = "") where T : class
        {
            Collection<T>(collectionName).DeleteOne(Builders<T>.Filter.Eq("_id", id));
        }

        public async Task DeleteAsync<T>(object id, string collectionName = "") where T : class
        {
            await Collection<T>(collectionName).DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }

        public void Delete<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            Collection<T>(collectionName).DeleteOne(Builders<T>.Filter.Eq("_id", val));
        }

        public async Task DeleteAsync<T>(T entity, string collectionName = "") where T : class
        {
            var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
            await Collection<T>(collectionName).DeleteOneAsync(Builders<T>.Filter.Eq("_id", val));
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            foreach (T entity in Collection<T>(collectionName).AsQueryable<T>().Where(predicate))
            {
                var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
                Delete<T>(val);
            }
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            foreach (T entity in Collection<T>(collectionName).AsQueryable<T>().Where(predicate))
            {
                var val = GetBsonIdProp(entity.GetType()).GetValue(entity, null);
                await DeleteAsync<T>(val);
            }
        }

        public void DeleteAll<T>(string collectionName = "") where T : class
        {
            Collection<T>(collectionName).DeleteMany(new BsonDocument());
        }

        public async Task DeleteAllAsync<T>(string collectionName = "") where T : class
        {
            await Collection<T>(collectionName).DeleteManyAsync(new BsonDocument());
        }
        #endregion

        public long Count<T>(string collectionName = "") where T : class
        {
            return List<T>(collectionName).LongCount();
        }

        public bool Exists<T>(Expression<Func<T, bool>> predicate, string collectionName = "") where T : class
        {
            return List<T>(collectionName).Any(predicate);
        }

        #region IQueryable<T>

        public IEnumerator<T> GetEnumerator<T>() where T : class
        {
            return List<T>().GetEnumerator();
        }
        #endregion


        private Expression<Func<T, bool>> GetByIdPredicate<T>(object value) where T : class
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
