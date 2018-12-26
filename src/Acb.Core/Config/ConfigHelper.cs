using Acb.Core.Config.Center;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Acb.Core.Config
{
    public class ConfigHelper
    {
        private IConfigurationRoot _config;
        private const string ConfigName = "appsettings.json";
        private IDisposable _callbackRegistration;
        private IConfigurationBuilder _builder;

        /// <summary> 配置文件变更事件 </summary>
        public event Action<object> ConfigChanged;

        /// <summary> 当前配置 </summary>
        public IConfiguration Config => _config;

        private ConfigHelper()
        {
            InitBuilder();
            InitConfig();
        }

        /// <summary> 单例模式 </summary>
        public static ConfigHelper Instance => Singleton<ConfigHelper>.Instance ??
                                             (Singleton<ConfigHelper>.Instance = new ConfigHelper());

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

        ///// <summary> 单例 </summary>
        //public static ConfigHelper Instance => Singleton<ConfigHelper>.Instance ??
        //                                       (Singleton<ConfigHelper>.Instance = new ConfigHelper());

        /// <summary> 构建配置 </summary>
        /// <param name="builderAction"></param>
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
            if (type.IsSimpleType())
                return _config.GetValue(key, defaultValue);
            //枚举类型处理
            if (type.IsEnum)
                return _config.GetValue<string>(key).CastTo(defaultValue);
            try
            {
                //区分大小写
                return _config.GetSection(key).Get<T>();
            }
            catch (Exception ex)
            {
                LogManager.Logger<ConfigHelper>().Error(ex.Message, ex);
                return defaultValue;
            }
        }

        /// <summary> 重新加载配置 </summary>
        public void Reload() { _config.Reload(); }

        /// <summary> 使用本地文件配置 </summary>
        public void UseLocal(string path = null)
        {
            var configPath = string.IsNullOrWhiteSpace(path) ? "configPath".Config<string>() : path;
            if (string.IsNullOrWhiteSpace(configPath))
                return;
            configPath = Path.Combine(Directory.GetCurrentDirectory(), configPath);
            if (!Directory.Exists(configPath))
                return;
            LogManager.Logger<CoreModule>().Info($"正在加载本地配置[{configPath}]");
            var jsons = Directory.GetFiles(configPath, "*.json");
            if (jsons.Any())
            {
                Build(b =>
                {
                    foreach (var json in jsons)
                    {
                        b.AddJsonFile(json, false, true);
                    }
                });
            }
        }

        /// <summary> 使用配置中心 </summary>
        public void UseCenter(CenterConfig config = null)
        {
            var provider = new ConfigCenterProvider(config);
            Build(b => b.Add(provider));
            ConfigChanged += provider.Reload;
        }
    }
}
