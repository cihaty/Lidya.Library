using MongoDB.Bson;
using MongoDB.Driver;

namespace Lidya.MongoRepository
{
    public class MongoDbContext
    {
        MongoClient client;
        public IMongoDatabase Database;

        #region Constructor

        public MongoDbContext(string connectionString, string dbName)
        {
            this.client = new MongoClient(new MongoUrl(connectionString));
            this.Database = this.client.GetDatabase(dbName);
        }
        public MongoDbContext(MongoClientSettings settings, string dbName)
        {
            this.client = new MongoClient(settings);
            this.Database = this.client.GetDatabase(dbName);
        }

        public MongoDbContext(IMongoDatabase mongoDatabase)
        {
            this.Database = mongoDatabase;
        }

        public MongoDbContext(MongoUrl MongoUrl)
        {
            this.client = new MongoClient(MongoUrl);
            this.Database = this.client.GetDatabase(MongoUrl.DatabaseName);
        }

        public IMongoCollection<BsonDocument> CustomCollection(string collectionName)
        {
            return Database.GetCollection<BsonDocument>(collectionName);
        }

        #endregion
    }
}
