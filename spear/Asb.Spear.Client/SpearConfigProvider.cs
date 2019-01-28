using Acb.Core.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Asb.Spear.Client
{
    internal class SpearConfigProvider : ConfigurationProvider, IConfigurationSource
    {
        private const string ArrayPattern = @"(\[[0-9]+\])*$";
        private readonly IDictionary<string, IDictionary<string, string>> _moduleDict;

        public SpearConfigProvider()
        {
            _moduleDict = new ConcurrentDictionary<string, IDictionary<string, string>>();
        }

        /// <summary> 键转换 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected internal virtual string ConvertKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            var split = key.Split('.');
            var sb = new StringBuilder();
            foreach (var part in split)
            {
                var keyPart = ConvertArrayKey(part);
                sb.Append(keyPart);
                sb.Append(ConfigurationPath.KeyDelimiter);
            }

            return sb.ToString(0, sb.Length - 1);
        }

        protected internal virtual string ConvertArrayKey(string key)
        {
            return Regex.Replace(key, ArrayPattern, match =>
            {
                var result = match.Value.Replace("[", ":").Replace("]", string.Empty);
                return result;
            });
        }

        public void LoadConfig(string module, object config)
        {
            if (!_moduleDict.TryGetValue(module, out var dict))
            {
                dict = new Dictionary<string, string>();
                _moduleDict.Add(module, dict);
            }
            dict.Clear();
            if (config != null)
            {
                var parser = new JsonConfigurationParser();
                var json = config is string ? config.ToString() : JsonConvert.SerializeObject(config);
                var t = parser.Parse(json);
                foreach (var key in t.Keys)
                {
                    var dKey = ConvertKey(key);
                    if (string.IsNullOrWhiteSpace(dKey))
                        continue;
                    dict[dKey] = t[key];
                }
            }

            //重新加载整体数据
            Data.Clear();
            foreach (var moduleData in _moduleDict)
            {
                foreach (var data in moduleData.Value)
                {
                    Data[data.Key] = data.Value;
                }
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this;
        }
    }
}
