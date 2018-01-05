using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Acb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Serialize
{
    internal class JsonContractResolver : DefaultContractResolver
    {
        private readonly string[] _props;
        private readonly bool _retain;
        private readonly NamingType _camelCase;

        public JsonContractResolver(NamingType camelCase)
        {
            _camelCase = camelCase;
        }

        /// <summary> 构造函数 </summary>
        /// <param name="camelCase">驼峰</param>
        /// <param name="retain">保留/排除：true为保留</param>
        /// <param name="props"></param>
        public JsonContractResolver(NamingType camelCase = NamingType.Normal, bool retain = true,
            params string[] props)
        {
            _camelCase = camelCase;
            _retain = retain;
            _props = props;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            switch (_camelCase)
            {
                case NamingType.CamelCase:
                    return propertyName.ToCamelCase();
                case NamingType.UrlCase:
                    return propertyName.ToUrlCase();
                default:
                    return base.ResolvePropertyName(propertyName);
            }
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var propList = base.CreateProperties(type, memberSerialization);
            if (_props == null || _props.Length == 0)
                return propList;
            return
                propList.Where(
                    p => _retain
                        ? _props.Contains(p.PropertyName)
                        : !_props.Contains(p.PropertyName))
                    .ToList();
        }
    }
}
