using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace Lidya.MongoRepository
{
    public class IntIdGenerator<T> : IIdGenerator
    {
        static int ID = 0;
        private Object thisLock = new Object();
        public object GenerateId(object container, object document)
        {
            lock (thisLock)
            {
                if (ID == 0)
                {
                    var col = (IMongoCollection<T>)container;
                    var sortBy = Builders<T>.Sort.Descending("_id");
                    var last = col.Find(Builders<T>.Filter.Empty).Sort(sortBy).Project(Builders<T>.Projection.Include("_id")).FirstOrDefault();
                    ID = (last == null) ? 1 : (last["_id"].AsInt32 + 1);
                }
                else
                {
                    ID = ID + 1;
                }
                return ID;
            }
        }

        public bool IsEmpty(object id)
        {
            return id.Equals(default(int));
        }
    }
}
