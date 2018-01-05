using Acb.Core.Dependency;
using Acb.Core.Modules;

namespace Acb.Core
{
    /// <summary> 初始化模块 </summary>
    public class CoreModule : DModule
    {
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;
            base.Initialize();
        }
    }
}
