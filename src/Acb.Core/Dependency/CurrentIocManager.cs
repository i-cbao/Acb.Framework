using System;

namespace Acb.Core.Dependency
{

    public static class CurrentIocManager
    {
        /// <summary> 依赖注入管理器 </summary>
        public static IIocManager IocManager { get; internal set; }

        /// <summary> 获取注入 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return IocManager.Resolve<T>();
        }

        /// <summary> 获取注入 </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static object Resolve(Type interfaceType)
        {
            return IocManager.Resolve(interfaceType);
        }

        public static bool IsRegisted(Type interfaceType)
        {
            return IocManager.IsRegistered(interfaceType);
        }
    }
}
