using Acb.Core.Extensions;
using System;

namespace Acb.Core.Config
{
    public static class ConfigExtensions
    {
        public static T Const<T>(this string key, T def = default(T))
        {
            return string.IsNullOrWhiteSpace(key) ? def : $"const:{key}".Config(def);
        }

        public static string Site(this string site)
        {
            return string.IsNullOrWhiteSpace(site) ? string.Empty : $"sites:{site}".Config<string>();
        }

        public static string Site(this Enum site)
        {
            return $"sites:{site}".Config<string>();
        }
    }
}
