using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Modules
{
    /// <summary> 模块扩展 </summary>
    public static class ModuleExtend
    {
        /// <summary> 是否是模块 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDModule(this Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   typeof(DModule).IsAssignableFrom(type);
        }

        /// <summary> 查找依赖的模块 </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static IList<Type> FindDependencyModules(this Type moduleType)
        {
            var dependencies = new List<Type>();
            if (!moduleType.IsDModule() || !moduleType.IsDefined(typeof(DependsOnAttribute), true))
                return dependencies;
            var dependsAttr =
                moduleType.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();

            foreach (var attribute in dependsAttr.Where(attribute => attribute.DependedModuleTypes != null))
            {
                dependencies.AddRange(attribute.DependedModuleTypes);
            }
            return dependencies;
        }
    }
}
