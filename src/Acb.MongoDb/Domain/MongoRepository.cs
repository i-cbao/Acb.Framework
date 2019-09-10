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

        /// <summary> 获取数据集合 </summary>
        protected IMongoCollection<T> Collection => _mongoHelper.Collection<T>();

        /// <summary> 新增数据 </summary>
        /// <param name="model"></param>
        public void Insert(T model)
        {
            Collection.InsertOne(model);
        }

        /// <summary> 批量新增 </summary>
        /// <param name="models"></param>
        public void InsertMany(IEnumerable<T> models)
        {
            Collection.InsertMany(models);
        }

        /// <summary> 根据键查询 </summary>
        /// <param name="value"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public T QueryById(object value, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            return Collection.Find(new BsonDocument
            {
                {keyColumn, BsonValue.Create(value)}
            }).FirstOrDefault();
        }

        /// <summary> 删除数据 </summary>
        /// <param name="value"></param>
        /// <param name="keyColumn"></param>
        public long Delete(object value, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            var result = Collection.DeleteOne(new BsonDocument
            {
                {keyColumn, BsonValue.Create(value)}
            });
            return result.DeletedCount;
        }
    }
}
