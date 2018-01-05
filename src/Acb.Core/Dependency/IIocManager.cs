using System;

namespace Acb.Core.Dependency
{
    public interface IIocManager : ILifetimeDependency
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
