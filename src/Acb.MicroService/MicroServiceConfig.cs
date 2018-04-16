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
        public string ConsulServer { get; set; }
        public bool ConsulCheck { get; set; }
        public int Consulinterval { get; set; } = 30;
        public int DeregisterAfter { get; set; } = 180;
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
