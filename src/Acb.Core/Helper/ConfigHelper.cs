using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Acb.Core.Helper
{
    public class ConfigHelper
    {
        private readonly IConfigurationRoot _config;
        private const string ConfigPrefix = "config:";
        private const string ConfigName = "appsettings.json";
        private IDisposable _callbackRegistration;

        public event Action<object> ConfigChanged;

        private ConfigHelper()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigName);
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            if (File.Exists(path))
            {
                _config = builder.AddJsonFile(ConfigName, false, true).Build();
            }
            else
            {
                _config = builder.Build();
            }
            _callbackRegistration = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, _config);
        }

        private void OnConfigChanged(object state)
        {
            ConfigChanged?.Invoke(state);
            _callbackRegistration?.Dispose();
            _callbackRegistration = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, state);
        }

        public static ConfigHelper Instance =
            Singleton<ConfigHelper>.Instance ?? (Singleton<ConfigHelper>.Instance = new ConfigHelper());


        ///// <summary>
        ///// 得到AppSettings中的配置字符串信息
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string Get(string key)
        //{
        //    return Get(string.Empty, supressKey: key);
        //}

        /// <summary> 配置文件读取 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">配置名</param>
        /// <param name="supressKey">配置别名</param>
        /// <returns></returns>
        public T Get<T>(T defaultValue = default(T), [CallerMemberName] string key = null,
              string supressKey = null)
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
                key = supressKey;
            key = $"{ConfigPrefix}{key}";
            var type = typeof(T);
            if (type.IsValueType || type == typeof(string))
                return _config.GetValue(key, defaultValue);

            var obj = Activator.CreateInstance<T>();
            _config.GetSection(key).Bind(obj);
            return obj;
        }
    }
}
