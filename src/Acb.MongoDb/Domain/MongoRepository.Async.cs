using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.MongoDb.Domain
{
    public partial class MongoRepository<T>
    {
        /// <summary> 新增 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task InsertAsync(T model)
        {
            await Collection.InsertOneAsync(model);
        }

        /// <summary> 批量新增 </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task InsertManyAsync(IEnumerable<T> models)
        {
            await Collection.InsertManyAsync(models);
        }

        /// <summary> 根据键查询 </summary>
        /// <param name="value"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public async Task<T> QueryByIdAsync(object value, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            return await Collection.Find(new BsonDocument
            {
                {keyColumn, BsonValue.Create(value)}
            }).FirstOrDefaultAsync();
        }

        public async Task<long> DeleteAsync(object value, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            var result = await Collection.DeleteOneAsync(new BsonDocument
            {
                {keyColumn, BsonValue.Create(value)}
            });
            return result.IsAcknowledged ? result.DeletedCount : 0;
        }
    }
}
