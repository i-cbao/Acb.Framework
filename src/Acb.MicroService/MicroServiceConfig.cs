﻿namespace Acb.MicroService
{
    public enum RegisterType
    {
        Redis,
        Consul
    }

    public class MicroServiceConfig
    {
        public RegisterType Register { get; set; }
        /// <summary> Consul服务地址 </summary>
        public string ConsulServer { get; set; }
        /// <summary> Consul令牌 </summary>
        public string ConsulToken { get; set; }
        public bool ConsulCheck { get; set; }
        public int Consulinterval { get; set; } = 30;
        public int DeregisterAfter { get; set; } = 180;
        public string Host { get; set; }
        public int Port { get; set; }
        /// <summary> 自动注销服务 </summary>
        public bool AutoDeregist { get; set; }
    }
}
