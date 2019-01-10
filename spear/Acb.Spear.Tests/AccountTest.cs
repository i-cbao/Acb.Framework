using Acb.Framework;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Account;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Acb.Core.Helper;
using Acb.Spear.Business.Domain;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.Tests
{
    [TestClass]
    public class AccountTest : DTest
    {
        private readonly IAccountContract _contract;

        public AccountTest()
        {
            _contract = Resolve<IAccountContract>();
        }

        [TestMethod]
        public void GuidTest()
        {
            var key = IdentityHelper.NewSequentialGuid().ToString("N");
            Print(key);
            var id = Guid.Parse(key);
            Print(id);
        }

        [TestMethod]
        public async Task MuliQueryTest()
        {
            var rep = Resolve<AccountRepository>();
            var t1 = rep.QueryAccountAsync("ichebao");
            var t2 = rep.QueryAccountAsync("icbhs");
            Print(await t1);
            Print(await t2);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            var dto = await _contract.CreateAsync(new AccountInputDto
            {
                Account = "icbhs",
                Nick = "i车保护神",
                Password = "123456",
                Role = AccountRole.Project,
                ProjectId = new Guid("d3e356f5-b686-c00e-e224-08d60ddebb59")
            });
            Assert.AreNotEqual(dto, null);
            Print(dto);
        }

        [TestMethod]
        public async Task LoginTest()
        {
            var dto = await _contract.LoginAsync("ichebao", "123456");
            Assert.AreNotEqual(dto, null);
            Print(dto);
        }

        [TestMethod]
        public async Task RecordsTest()
        {
            var list = await _contract.LoginRecordsAsync(new Guid("b5ff8cc7-2100-ced2-d13a-08d67552b2a4"), 1, 10);
            Assert.AreNotEqual(list, null);
            Print(list);
        }
    }
}
