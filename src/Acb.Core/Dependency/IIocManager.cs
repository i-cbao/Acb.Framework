using System;

namespace Acb.Core.Dependency
{
    /// <summary> 依赖注入管理器 </summary>
    public interface IIocManager : ILifetimeDependency
    {
        /// <summary> 获取注入 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary> 获取注入 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary> 是否注册注入 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsRegistered(Type type);
    }
}
