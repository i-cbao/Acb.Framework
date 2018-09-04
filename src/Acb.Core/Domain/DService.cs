using Acb.Core.Dependency;
using System;

namespace Acb.Core.Domain
{
    public abstract class DService
    {
        protected T Resolve<T>()
        {
            return CurrentIocManager.Resolve<T>();
        }

        protected object Resolve(Type interfaceType)
        {
            return CurrentIocManager.Resolve(interfaceType);
        }
    }
}
