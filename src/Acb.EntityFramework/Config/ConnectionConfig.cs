using System;

namespace Acb.EntityFramework.Config
{
    [Serializable]
    public class ConnectionConfig
    {
        /// <summary> 连接名称 </summary>
        public string Name { get; set; }

        /// <summary> 数据库驱动名称 </summary>
        public string ProviderName { get; set; } = "MySql";

        /// <summary> 连接字符串 </summary>
        public string ConnectionString { get; set; }
    }
}
