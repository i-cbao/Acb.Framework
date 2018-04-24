using Acb.AutoMapper;
using Acb.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class AutoMapperTest : DTest
    {
        public class MyClass
        {
            public string MyName { get; set; }
            private string _url;

            public string Url
            {
                get => _url;
                set => _url = value.FullUrl("sites:file".Config<string>());
            }

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
                //url = "data:image/png;base64,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",
                url = "test.jpg",
                Cls = new MyClass2 { Age = 20 }
            };
            var m = t.MapTo<MyClass>(MapperType.FromUrl);
            Print(m);
        }
    }
}
