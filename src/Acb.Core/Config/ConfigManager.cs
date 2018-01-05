using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using System.Collections.Concurrent;
using System.IO;

namespace Acb.Core.Config
{
    /// <summary>
    /// 配置文件管理
    /// </summary>
    public class ConfigManager
    {
        private readonly ConcurrentDictionary<string, object> _configCache;

        private readonly string _configPath;

        private ConfigManager()
        {
            _configPath = "configPath".Config(string.Empty);
            _configCache = new ConcurrentDictionary<string, object>();
            _configPath = _configPath.GetPath();
            if (!Directory.Exists(_configPath))
                throw new DException($"{_configPath}不存在");
            //文件监控
            var watcher = new FileSystemWatcher(_configPath)
            {
                IncludeSubdirectories = true,
                Filter = "*.config", //"*.config|*.xml"多个扩展名不受支持！
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size
            };
            watcher.Changed += Reset;
            watcher.Deleted += Reset;
            watcher.Renamed += Reset;
            watcher.Created += Reset;
            watcher.EnableRaisingEvents = true;
        }

        public static ConfigManager Instance
            => Singleton<ConfigManager>.Instance ?? (Singleton<ConfigManager>.Instance = new ConfigManager());

        public T GetConfig<T>(string fileName)
            where T : ConfigBase
        {
            if (_configCache.ContainsKey(fileName))
            {
                return _configCache[fileName].CastTo<T>();
            }
            var path = Path.Combine(_configPath, fileName);
            if (!File.Exists(path))
                return null;
            var config = XmlHelper.XmlDeserialize<T>(path);
            _configCache.TryAdd(fileName, config);
            return config;
        }

        public void SetConfig<T>(string fileName, T config)
            where T : ConfigBase
        {
            var path = Path.Combine(_configPath, fileName);
            XmlHelper.XmlSerialize(path, config);
        }

        private void Reset(object sender, FileSystemEventArgs e)
        {
            if (_configCache.ContainsKey(e.Name))
            {
                object config;
                _configCache.TryRemove(e.Name, out config);
            }
            Change?.Invoke(e.Name);
        }

        /// <summary> 配置文件改变委托 </summary>
        /// <param name="fileName"></param>
        public delegate void ConfigChange(string fileName);

        /// <summary> 配置文件改变事件 </summary>
        public event ConfigChange Change;
    }
}
