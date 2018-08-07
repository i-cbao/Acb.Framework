namespace Acb.EventBus.RabbitMq
{
    public class RabbitMqConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Broker { get; set; }
        public string VirtualHost { get; set; }
    }
}
