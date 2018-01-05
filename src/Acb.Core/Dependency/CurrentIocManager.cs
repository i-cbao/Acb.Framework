using System;

namespace Acb.Core.Dependency
{
    public static class CurrentIocManager
    {
        public static IIocManager IocManager { get; internal set; }

        public static T Resolve<T>()
        {
            return IocManager.Resolve<T>();
        }

        public static object Resolve(Type interfaceType)
        {
            return IocManager.Resolve(interfaceType);
        }
    }
}
