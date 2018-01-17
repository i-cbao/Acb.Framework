using System;
using System.Collections.Generic;

namespace Acb.MongoDb
{
    /// <summary>
    /// MongoDB配置文件
    /// </summary>
    [Serializable]
    public class MongoConfig
    {
        /// <summary> 服务器列表 </summary>
        public List<DServer> Servers { get; set; }

        /// <summary> 数据库认证列表 </summary>
        public List<DCredential> Credentials { get; set; }

        /// <summary> 超时时间 (s) </summary>
        public int Timeout { get; set; }

        public MongoConfig()
        {
            Servers = new List<DServer>();
            Credentials = new List<DCredential>();
        }
    }

    [Serializable]
    public class DServer
    {
        /// <summary> Ip </summary>
        public string Host { get; set; }

        /// <summary> 端口号 </summary>
        public int Port { get; set; }
    }

    /// <summary> 身份认证 </summary>
    [Serializable]
    public class DCredential
    {
        public string Database { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
    }
}
