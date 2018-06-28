
using Acb.Core.Dependency;

namespace Acb.Core.Modules
{
    /// <summary> 模块管理接口 </summary>
    public interface IModuleManager : IScopedDependency
    {
        /// <summary> 加载所有模块 </summary>
        void InitializeModules();

        /// <summary> 关闭所有模块 </summary>
        void ShutdownModules();
    }
}
