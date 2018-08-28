using Acb.Core.Extensions;

namespace Acb.RocketMq
{
    public class RocketMqConfig
    {
        private const string Region = "rocketmq";
        private const string DefaultName = "default";

        public static RocketMqConfig Config(string name = null)
        {
            name = string.IsNullOrWhiteSpace(name) ? DefaultName : name;
            return $"{Region}:{name}".Config<RocketMqConfig>();
        }
        /// <summary> 服务地址 </summary>
        public string Host { get; set; }
        /// <summary> 主题 </summary>
        public string Topic { get; set; }
        /// <summary> 子表达式 </summary>
        public string SubExpression { get; set; } = "*";
        /// <summary> Access密钥 </summary>
        public string AccsessKey { get; set; }
        public string SecretKey { get; set; }
        /// <summary> 生产者ID </summary>
        public string ProducerId { get; set; }
        /// <summary> 消费者ID </summary>
        public string ConsumerId { get; set; }
    }
}
