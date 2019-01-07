using Acb.Core.Config;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Helper.Http;
using Acb.Core.Modules;
using System;
using System.Text;

namespace Acb.Core
{
    /// <inheritdoc />
    /// <summary> 初始化模块 </summary>
    public class CoreModule : DModule
    {
        /// <inheritdoc />
        /// <summary> 初始化 </summary>
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;
            //编码注册
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //中心配置
            var configHelper = ConfigHelper.Instance;
            configHelper.UseLocal();
            configHelper.UseCenter();
            base.Initialize();
        }

        /// <inheritdoc />
        /// <summary> 应用程序关闭 </summary>
        public override void Shutdown()
        {
            HttpHelper.Instance.Dispose();
            if (IocManager.IsRegistered<IEventBus>())
            {
                var bus = IocManager.Resolve<IEventBus>();
                if (bus is IDisposable dis)
                    dis.Dispose();
            }
            base.Shutdown();
        }
    }
}
