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
            LogManager.AddAdapter(new ConsoleAdapter());
            Bootstrap = DBootstrap.Instance;
            Bootstrap.Initialize();
            //LogManager.ClearAdapter();

        }

        /// <summary> 打印数据 </summary>
        /// <param name="result"></param>
        protected void Print(object result)
        {
            if (result == null)
            {
                Console.WriteLine("NULL");
                return;
            }
            var type = result.GetType();
            if (type.IsSimpleType())
                Console.WriteLine(result);
            else
            {
                string json;
                try
                {
                    json = JsonHelper.ToJson(result, NamingType.CamelCase, true);
                }
                catch
                {
                    json = result.ToString();
                }

                Console.WriteLine(json);
            }
        }
    }
}
