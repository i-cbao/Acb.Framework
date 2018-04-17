using Acb.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class AutoMapperTest : DTest
    {
        public class MyClass
        {
            public string MyName { get; set; }
            public MyClass2 Cls { get; set; }
        }

        public class MyClass2
        {
            public int Age { get; set; }
            public string From { get; set; }
        }

        [TestMethod]
        public void MapperTest()
        {
            var t = new
            {
                my_name = "shay",
                Cls = new MyClass2 { Age = 20 }
            };
            var m = t.MapTo<MyClass>(MapperType.ToUrl);
            Print(m);
        }
    }
}
