using Acb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.Core.Reflection
{
    public class DefaultTypeFinder : ITypeFinder
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public Type[] Find(Func<Type, bool> expression)
        {
            return FindTypes(expression).ToArray();
        }

        public Type[] FindAll()
        {
            return FindTypes().ToArray();
        }

        private List<Type> FindTypes(Func<Type, bool> expression = null)
        {
            if (expression == null)
                expression = t => true;
            var types = new List<Type>();
            foreach (var assembly in AssemblyFinder.FindAll().Distinct())
            {
                List<Type> list;

                try
                {
                    list = assembly.GetTypes().Where(expression).ToList();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    list = ex.Types.Where(expression).ToList();
                }
                if (list.IsNullOrEmpty())
                    continue;
                types.AddRange(list.Where(t => t != null));
            }
            return types;
        }
    }
}
