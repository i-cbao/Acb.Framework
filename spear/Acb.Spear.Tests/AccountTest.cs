using Acb.Framework;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Account;
using Acb.Spear.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

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
        public async Task CreateTest()
        {
            var dto = await _contract.CreateAsync(new AccountInputDto
            {
                Account = "ichebao",
                Nick = "°®³µ±£",
                Password = "123456",
                Role = AccountRole.Project,
                ProjectId = new Guid("5afba2f4-9077-c91c-963d-08d5fbcacf01")
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
