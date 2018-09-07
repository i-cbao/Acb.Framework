using Acb.Core.Dependency;
using System;

namespace Acb.Core.Reflection
{
    /// <summary> 类型查找器 </summary>
    public interface ITypeFinder : ISingleDependency
    {
        Type[] Find(Func<Type, bool> expression);

        Type[] FindAll();
    }
}
