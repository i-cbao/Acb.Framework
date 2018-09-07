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

        /// <summary> 获取注入 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>(string key)
        {
            return IocManager.Resolve<T>(key);
        }

        /// <summary> 获取注入 </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(string key, Type type)
        {
            return IocManager.Resolve(key, type);
        }

        public static bool IsRegisted(Type interfaceType)
        {
            return IocManager.IsRegistered(interfaceType);
        }

        /// <summary> 是否注册注入 </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsRegistered(string key, Type type)
        {
            return IocManager.IsRegistered(key, type);
        }

        /// <summary> 是否注册注入 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsRegistered<T>(string key)
        {
            return IocManager.IsRegistered<T>(key);
        }

        /// <summary> 是否注册注入 </summary>
        /// <returns></returns>
        public static bool IsRegistered<T>()
        {
            return IocManager.IsRegistered<T>();
        }
    }
}
