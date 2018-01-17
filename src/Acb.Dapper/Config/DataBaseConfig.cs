using System;

namespace Acb.Dapper.Config
{
    ///// <summary> 数据库连接配置 </summary>
    //[Serializable]
    //public class DataBaseConfig
    //{
    //    public List<ConnectionConfig> Connections { get; set; }

    //    public DataBaseConfig()
    //    {
    //        Connections = new List<ConnectionConfig>();
    //    }

    //    public ConnectionConfig Get(string name)
    //    {
    //        var item =
    //            Connections.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
    //        //if (item != null && item.IsEncrypt)
    //        //{
    //        //    item.ConnectionString = SecurityHelper.Decode(item.ConnectionString);
    //        //}
    //        return item;
    //    }
    //}

    [Serializable]
    public class ConnectionConfig
    {
        public string Name { get; set; }

        //[XmlAttribute("is_encrypt")]
        //public bool IsEncrypt { get; set; }
        public string ProviderName { get; set; } = "SqlServer";
        public string ConnectionString { get; set; }
    }
}
