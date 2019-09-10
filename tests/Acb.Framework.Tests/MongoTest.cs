using Acb.Core.Domain.Entities;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Acb.MongoDb;
using Acb.MongoDb.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [Naming("m_test")]
    public class MTest : BaseEntity<string>
    {
        public override string Id { get; set; }

        public Guid ReceiveId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class MTestRepository : MongoRepository<MTest>
    {
        public MTest QueryByName(string name)
        {
            var filter = Builders<MTest>.Filter.Eq(nameof(MTest.Name), name);
            return Collection.Find(filter).FirstOrDefault();
        }
    }

    [TestClass]
    public class MongoTest : DTest
    {
        private const string DbName = "acb_test";

        public MongoTest()
        {
            LogManager.LogLevel(LogLevel.Info);
        }

        protected override void MapServices(IServiceCollection services)
        {
            //services.AddMongoDb(database: DbName);
            base.MapServices(services);
        }

        [TestMethod]
        public void Test()
        {
            var repo = Resolve<MTestRepository>();
            repo.Insert(new MTest
            {
                Id = IdentityHelper.Guid32,
                Name = "shay01",
                ReceiveId = IdentityHelper.NewSequentialGuid(),
                Age = 18,
                CreationTime = Clock.Now
            });
            //var m = repo.QueryByName("shay");
            var m = repo.QueryById("shay01", nameof(MTest.Name));
            Print(m);
            return;
            var helper = Resolve<MongoHelper>();
            var result = CodeTimer.Time("mongo test", 10, () =>
            {
                try
                {
                    var clt = helper.Collection<MTest>();
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
            }, 5);
            Print(result.ToString());
        }

        [TestMethod]
        public async Task BsonTest()
        {
            var helper = Resolve<MongoHelper>();
            var clt = helper.BsonCollection<MTest>();
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

            var info = helper.Database.RunCommand<BsonDocument>(new BsonDocument("buildinfo", 1));
            Print(info);
        }
    }
}
