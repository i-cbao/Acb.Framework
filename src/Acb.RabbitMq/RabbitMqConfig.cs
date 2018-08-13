using Acb.Core.Extensions;

namespace Acb.RabbitMq
{
    public class RabbitMqConfig
    {
        private const string Region = "rabbit";
        private const string DefaultName = "default";

        public static RabbitMqConfig Config(string name = null)
        {
            name = string.IsNullOrWhiteSpace(name) ? DefaultName : name;
            return $"{Region}:{name}".Config<RabbitMqConfig>();
        }

        public string Host { get; set; }
        public int Port { get; set; } = -1;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Broker { get; set; }
        public string VirtualHost { get; set; } = "/";
    }
}
