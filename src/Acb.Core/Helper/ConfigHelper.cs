using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Acb.Core.Helper
{
    public class ConfigHelper
    {
        private IConfigurationRoot _config;
        private const string ConfigName = "appsettings.json";
        private IDisposable _callbackRegistration;
        private IConfigurationBuilder _builder;

        public event Action<object> ConfigChanged;

        public IConfiguration Config => _config;

        private ConfigHelper()
        {
            InitBuilder();
            InitConfig();
        }

        private void InitBuilder()
        {
            var currentDir = Directory.GetCurrentDirectory();
            _builder = new ConfigurationBuilder().SetBasePath(currentDir);
            var path = Path.Combine(currentDir, ConfigName);
            if (File.Exists(path))
            {
                _builder.AddJsonFile(ConfigName, false, true);
            }
        }

        private void InitConfig()
        {
            _config = _builder.Build();
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

        public void Build(Action<IConfigurationBuilder> builderAction)
        {
            builderAction.Invoke(_builder);
            var sources = _builder.Sources.Reverse().ToArray();
            //倒序排列，解决读取配置时的优先级问题
            for (var i = 0; i < sources.Length; i++)
            {
                _builder.Sources[i] = sources[i];
            }

            _config = _builder.Build();
        }

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
            var type = typeof(T);
            if (type.IsValueType || type == typeof(string))
                return _config.GetValue(key, defaultValue);
            var obj = Activator.CreateInstance<T>();
            _config.GetSection(key).Bind(obj);
            return obj;
        }

        public void Reload() { _config.Reload(); }
    }
}
