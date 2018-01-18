using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Framework.Logging;
using System;

namespace Acb.Framework
{
    public abstract class DTest
    {
        protected DBootstrap Bootstrap;

        protected DTest()
        {
            Bootstrap = DBootstrap.Instance;
            Bootstrap.Initialize();
            //LogManager.ClearAdapter();
            LogManager.AddAdapter(new ConsoleAdapter());
        }

        protected void Print<T>(T result)
        {
            var type = typeof(T);
            if (type.IsValueType || type == typeof(string))
                Console.WriteLine(result);
            else
                Console.WriteLine(JsonHelper.ToJson(result, NamingType.CamelCase, true));
        }
    }
}
