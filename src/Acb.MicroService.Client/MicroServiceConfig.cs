namespace Acb.MicroService.Client
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
    }
}
