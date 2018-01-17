using System;
using System.Collections.Generic;

namespace Acb.Redis
{
    /// <summary> Redis连接配置 </summary>
    [Serializable]
    public class RedisConfig
    {
        /// <summary> Redis连接信息集 </summary>
        public List<RedisConnectionItem> Connections { get; set; }

        /// <summary> Redis连接配置 </summary>
        public RedisConfig()
        {
            Connections = new List<RedisConnectionItem>();
        }
    }

    /// <summary>
    /// Redis连接属性
    /// </summary>
    [Serializable]
    public class RedisConnectionItem
    {
        /// <summary>  IP </summary>
        public string Ip { get; set; }

        /// <summary> 端口 </summary>
        public int Port { get; set; }

        /// <summary> 密码 </summary>
        public string Password { get; set; }
    }
}
