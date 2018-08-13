using Acb.Core.Extensions;

namespace Acb.RocketMq
{
    public class RocketMqConfig
    {
        private const string Region = "rocket";
        private const string DefaultName = "default";

        public static RocketMqConfig Config(string name = null)
        {
            name = string.IsNullOrWhiteSpace(name) ? DefaultName : name;
            return $"{Region}:{name}".Config<RocketMqConfig>();
        }
    }
}
