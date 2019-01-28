using Acb.Framework;
using Acb.Spear.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Tests
{
    [TestClass]
    public class DatabaseTest : DTest
    {
        private readonly IDatabaseContract _contract;

        public DatabaseTest()
        {
            _contract = Resolve<IDatabaseContract>();
        }

        [TestMethod]
        public async Task TablesTest()
        {
            var db = await _contract.GetAsync(new Guid("b5ff8cc7-2100-ced2-d13a-08d67552b2a1"));
            Print(db);
        }
    }
}
