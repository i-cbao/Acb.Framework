using Acb.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Acb.Dapper.Config
{
    /// <summary> 数据库连接配置 </summary>
    [Serializable]
    [XmlRoot("database")]
    [FileName("database.config")]
    public class DataBaseConfig : ConfigBase
    {
        [XmlArray("connections")]
        [XmlArrayItem("item")]
        public List<ConnectionConfig> Connections { get; set; }

        public DataBaseConfig()
        {
            Connections = new List<ConnectionConfig>();
        }

        public ConnectionConfig Get(string name)
        {
            var item =
                Connections.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
            //if (item != null && item.IsEncrypt)
            //{
            //    item.ConnectionString = SecurityHelper.Decode(item.ConnectionString);
            //}
            return item;
        }
    }

    [Serializable]
    public class ConnectionConfig
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        //[XmlAttribute("is_encrypt")]
        //public bool IsEncrypt { get; set; }

        [XmlAttribute("providerName")]
        public string ProviderName { get; set; } = "SqlServer";

        [XmlText]
        public string ConnectionString { get; set; }
    }
}
