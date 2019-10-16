using Acb.Core.Extensions;
using Acb.Core.Serialize;
using Acb.Framework.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Framework
{
    /// <summary> 测试基类 </summary>
    public abstract class DTest
    {
        /// <summary> 项目启动项 </summary>
        protected DBootstrap Bootstrap;

        protected virtual void MapServices(ContainerBuilder builder)
        {
        }

        protected virtual void MapServices(IServiceCollection services)
        {

        }

        protected virtual void UseServices(IServiceProvider provider)
        {

        }

        /// <summary> 默认构造函数 </summary>
        protected DTest()
        {
            Init();
        }

        private void Init()
        {
            var services = new ServiceCollection();
            services.AddSystemLogging();
            MapServices(services);
            Bootstrap = new DBootstrap();
            Bootstrap.BuilderHandler += builder =>
            {
                builder.Populate(services);
                MapServices(builder);
            };
            Bootstrap.Initialize();
            var container = Bootstrap.CreateContainer();
            var provider = new AutofacServiceProvider(container);
            UseServices(provider);
        }

        protected T Resolve<T>()
        {
            return Bootstrap.ContainerRoot.Resolve<T>();
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
