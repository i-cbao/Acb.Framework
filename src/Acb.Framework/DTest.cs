using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Framework.Logging;
using System;

namespace Acb.Framework
{
    /// <summary> 测试基类 </summary>
    public abstract class DTest
    {
        /// <summary> 项目启动项 </summary>
        protected DBootstrap Bootstrap;

        /// <summary> 默认构造函数 </summary>
        protected DTest()
        {
            Bootstrap = DBootstrap.Instance;
            Bootstrap.Initialize();
            //LogManager.ClearAdapter();
            LogManager.AddAdapter(new ConsoleAdapter());
        }

        /// <summary> 打印数据 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        protected void Print<T>(T result)
        {
            var type = typeof(T);
            if (type.IsSimpleType())
                Console.WriteLine(result);
            else
                Console.WriteLine(JsonHelper.ToJson(result, NamingType.CamelCase, true));
        }
    }
}
