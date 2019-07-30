using Acb.Core.Config;
using Acb.Core.Domain.Entities;
using Acb.Core.Domain.Repositories;
using Acb.Core.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Acb.MongoDb.Domain
{
    public partial class MongoRepository<T> : IRepository where T : IEntity
    {
        private readonly MongoHelper _mongoHelper;

        public MongoRepository(MongoHelper mongoHelper = null)
        {
            if (mongoHelper == null)
            {
                var dbName = "mongo_db".Const<string>();
                if (string.IsNullOrWhiteSpace(dbName))
                    dbName = typeof(T).PropName();
                mongoHelper = MongoManager.Instance.GetHelper(dbName);
            }
            _mongoHelper = mongoHelper;
        }

        protected IMongoCollection<T> Collection => _mongoHelper.Collection<T>();

        public void Insert(T model)
        {
            Collection.InsertOne(model);
        }

        public void InsertMany(IEnumerable<T> models)
        {
            Collection.InsertMany(models);
        }

        public T QueryById(object id, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            return Collection.Find(new BsonDocument
            {
                {keyColumn, BsonValue.Create(id)}
            }).FirstOrDefault();
        }
    }
}
