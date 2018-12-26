using Acb.ProxyGenerator.Attributes;
using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Acb.ProxyGenerator.Utils
{
    internal static class PropertyInjectionUtils
    {
        private static readonly ConcurrentDictionary<Type, bool> Dictionary = new ConcurrentDictionary<Type, bool>();

        public static bool TypeRequired(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return Dictionary.GetOrAdd(type, _ => type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.CanWrite).Any(x => x.GetReflector().IsDefined<FromContainerAttribute>()));
        }

        public static bool Required(object instance)
        {
            if (instance == null)
            {
                return false;
            }
            return TypeRequired(instance.GetType());
        }
    }
}