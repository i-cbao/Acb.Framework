using Acb.Core.Domain.Entities;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Tests;
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
            LogManager.LogLevel(LogLevel.Info);
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
        public void Test()
        {

            var result = CodeTimer.Time("mongo test", 10, () =>
            {
                try
                {
                    var clt = _helper.Collection<MTest>();
                    var builder = Builders<MTest>.Filter;
                    var updater = Builders<MTest>.Update;

                    clt.UpdateOne(builder.Eq(nameof(MTest.Id), "79074105e587cfd0ab4308d5d2d0d4ad"),
                        updater.Set(nameof(MTest.Age), 18));

                    var filters = builder.Empty;
                    clt.Find(filters);
                }
                catch (Exception ex)
                {
                    Print(ex.Format());
                    throw;
                }
            }, 100);
            Print(result.ToString());
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
