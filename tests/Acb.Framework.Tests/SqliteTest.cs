using Acb.Core.Domain.Entities;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.SQLite;
using Acb.Dapper.SQLite.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Acb.Framework.Tests
{
    public class TUser : BaseEntity<string>
    {
        [Dapper.SQLite.Attributes.Key, Required]
        public override string Id { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public int Role { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
    }

    [TestClass]
    public class SqliteTest : DTest
    {
        private static readonly string DbPath = Path.Combine(Directory.GetCurrentDirectory(), "acb_sqlite.db3");
        [TestMethod]
        public void AutoCreateTest()
        {
            DbPath.AutoCreateDb();
        }

        [TestMethod]
        public void ConnectTest()
        {
            using (var conn = DbPath.CreateConnection())
            {
                conn.Open();
                //var result = conn.Insert(new TUser
                //{
                //    Id = IdentityHelper.Guid32,
                //    Mobile = "13562545845",
                //    CreateTime = Clock.Now,
                //    Role = 2
                //});
                //Print(result);
                var users = conn.QueryAll<TUser>();
                Print(users);
            }
        }
    }
}
