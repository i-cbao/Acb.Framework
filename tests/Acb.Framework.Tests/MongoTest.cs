using Acb.Core.Domain.Entities;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.MongoDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class MongoTest : DTest
    {
        private const string DbName = "acb_test";
        private readonly MongoHelper _helper;

        public MongoTest()
        {
            _helper = MongoManager.Instance.GetHelper(DbName);
        }

        public class MTest : BaseEntity<string>
        {
            public override string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime CreationTime { get; set; }
        }

        [TestMethod]
        public async Task Test()
        {
            var clt = _helper.Collection<MTest>();

            await clt.InsertOneAsync(
                new MTest { Id = IdentityHelper.Guid32, Name = "nice", Age = 22, CreationTime = Clock.Now });

            //var builder = Builders<MTest>.Filter;
            //var updater = Builders<MTest>.Update;

            //var result = await clt.UpdateOneAsync(builder.Eq(nameof(MTest.Id), "9c8ee318f694c0118ea108d57f58df4a"),
            //    updater.Set(nameof(MTest.Age), 18));
            //Print(result);

            //var filters = builder.Empty;
            //var model = await clt.FindAsync(filters);
            //Print(model.ToList());
        }

        [TestMethod]
        public async Task BsonTest()
        {
            var clt = _helper.BsonCollection<MTest>();
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var models = await clt.Find(new BsonDocument()).Project(projection).ToListAsync();
            var list = models.ToList();
            //var json = JsonConvert.SerializeObject(list);
            list.ForEach(Print);

            //var updates = new WriteModel<BsonDocument>[]
            //{
            //    new InsertOneModel<BsonDocument>(new BsonDocument("_id", 3)),
            //    new DeleteOneModel<BsonDocument>(new BsonDocument("_id", 3)),
            //    new UpdateOneModel<BsonDocument>(new BsonDocument(), new BsonDocument())
            //};

            //var result = await clt.BulkWriteAsync(updates, new BulkWriteOptions { IsOrdered = true });
            //Print(result);

            var info = _helper.Database.RunCommand<BsonDocument>(new BsonDocument("buildinfo", 1));
            Print(info);
        }
    }
}
