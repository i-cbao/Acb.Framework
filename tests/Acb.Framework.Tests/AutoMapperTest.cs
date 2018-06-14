using Acb.Core.Domain.Dtos;
using Acb.Core.Domain.Entities;
using Acb.Core.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class AutoMapperTest : DTest
    {
        public class TUser : BaseEntity<Guid>
        {
            public override Guid Id { get; set; }
        }

        public class UserDto : DDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void Test()
        {
            var model = new TUser[] { new TUser { Id = IdentityHelper.NewSequentialGuid() } };
            var dto = model.MapToList<UserDto>();
            Print(dto);
        }
    }
}
