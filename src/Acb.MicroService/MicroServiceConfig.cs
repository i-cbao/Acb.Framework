namespace Acb.MicroService
{
    internal enum RegisterType
    {
        Redis,
        Consul
    }

    internal class MicroServiceConfig
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
    }
}
