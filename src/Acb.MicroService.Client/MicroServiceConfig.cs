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
        public string ConsulServer { get; set; }
    }
}
