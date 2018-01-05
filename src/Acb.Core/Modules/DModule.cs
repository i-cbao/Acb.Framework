using Acb.Core.Dependency;

namespace Acb.Core.Modules
{
    /// <summary> 应用启动依赖模块基类 </summary>
    public abstract class DModule : ILifetimeDependency
    {
        /// <summary> 依赖管理 </summary>
        protected internal IIocManager IocManager { get; internal set; }

        /// <summary> 初始化前置操作 </summary>
        public virtual void PreInitialize()
        {

        }

        /// <summary> 初始化模块 </summary>
        public virtual void Initialize()
        {

        }

        /// <summary> 初始化完成 </summary>
        public virtual void PostInitialize()
        {

        }

        /// <summary> 关闭模块 </summary>
        public virtual void Shutdown()
        {

        }
    }
}
