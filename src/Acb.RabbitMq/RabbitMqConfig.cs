using Acb.Core.Dependency;
using Acb.Core.Extensions;

namespace Acb.RabbitMq
{
    public class RabbitMqConfig : ISingleDependency
    {
        private const string Region = "rabbit";

        private static T Config<T>(string name)
        {
            return $"{Region}:{name}".Config<T>();
        }

        public string Host => Config<string>("host");
        public int Port => Config<int>("port");
        public string UserName => Config<string>("user");
        public string Password => Config<string>("password");
        public string Broker => Config<string>("broker");
    }
}
