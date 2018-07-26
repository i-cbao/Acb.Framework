using Acb.Core.Dependency;
using Acb.Core.Extensions;

namespace Acb.RabbitMq
{
    public class RabbitMqConfig : ISingleDependency
    {
        private const string Region = "rabbit";

        private static T Config<T>(string name, T def = default(T))
        {
            return $"{Region}:{name}".Config(def);
        }

        public string Host => Config<string>("host");
        public int Port => Config("port", -1);
        public string UserName => Config<string>("user");
        public string Password => Config<string>("password");
        public string Broker => Config("broker", "icb_broker");
        public string VirtualHost => Config("virtualHost", "/");
    }
}
