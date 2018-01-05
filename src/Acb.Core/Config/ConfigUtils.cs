using System;
using System.Linq;
using System.Xml.Serialization;

namespace Acb.Core.Config
{
    /// <summary> 配置文件辅助类 </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConfigUtils<T>
        where T : ConfigBase
    {
        private string _fileName;

        private readonly ConfigManager _configManager;

        private ConfigUtils()
        {
            _configManager = ConfigManager.Instance;
        }

        /// <summary> 单例 </summary>
        public static ConfigUtils<T> Instance
            => Singleton<ConfigUtils<T>>.Instance ?? (Singleton<ConfigUtils<T>>.Instance = new ConfigUtils<T>());

        /// <summary> 获取当前配置 </summary>
        public static T Config => Instance.Get();


        /// <summary> 文件名 </summary>
        [XmlIgnore]
        public string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(_fileName))
                    return _fileName;
                var attrs = typeof(T).GetCustomAttributes(typeof(FileNameAttribute), true);
                if (!attrs.Any())
                    return _fileName;
                var attr = attrs.FirstOrDefault() as FileNameAttribute;
                if (attr != null) _fileName = attr.Name;
                return _fileName;
            }
        }

        /// <summary> 获取配置文件 </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T Get(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            return _configManager.GetConfig<T>(fileName);
        }


        /// <summary> 更新配置文件 </summary>
        /// <param name="config"></param>
        /// <param name="fileName"></param>
        public void Set(T config, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            _configManager.SetConfig(fileName, config);
        }
    }
}
