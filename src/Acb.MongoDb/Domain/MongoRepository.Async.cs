using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.MongoDb.Domain
{
    public partial class MongoRepository<T>
    {
        public async Task InsertAsync(T model)
        {
            await Collection.InsertOneAsync(model);
        }

        public async Task InsertManyAsync(IEnumerable<T> models)
        {
            await Collection.InsertManyAsync(models);
        }

        public async Task<T> QueryByIdAsync(object id, string keyColumn = null)
        {
            keyColumn = string.IsNullOrWhiteSpace(keyColumn) ? "_id" : keyColumn;
            return await Collection.Find(new BsonDocument
            {
                {keyColumn, BsonValue.Create(id)}
            }).FirstOrDefaultAsync();
        }
    }
}
