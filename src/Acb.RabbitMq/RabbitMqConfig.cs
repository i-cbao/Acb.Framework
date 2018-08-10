using Acb.Core.Extensions;

namespace Acb.RabbitMq
{
    public class RabbitMqConfig
    {
        private const string Region = "rabbit";

        private static T Config<T>(string name, T def = default(T))
        {
            return $"{Region}:{name}".Config(def);
        }

        public RabbitMqConfig()
        {
            Host = Config<string>("host");
            Port = Config("port", -1);
            UserName = Config<string>("user");
            Password = Config<string>("password");
            Broker = Config("broker", "icb_broker");
            VirtualHost = Config("virtualHost", "/");
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Broker { get; set; }
        public string VirtualHost { get; set; }
    }
}
