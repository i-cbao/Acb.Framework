using Acb.Core.Dependency;
using Acb.Core.Logging;
using System;

namespace Acb.Core.Domain
{
    public abstract class DService
    {
        protected ILogger Logger => LogManager.Logger(GetType());

        protected T Resolve<T>()
        {
            return CurrentIocManager.Resolve<T>();
        }

        protected object Resolve(Type interfaceType)
        {
            return CurrentIocManager.Resolve(interfaceType);
        }

        protected object Resolve(string key, Type interfaceType)
        {
            return CurrentIocManager.Resolve(key, interfaceType);
        }
    }
}
