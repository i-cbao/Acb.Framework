using System;

namespace Acb.RocketMq
{
    public class DefaultRocketMqConnection : IRocketMqConnection
    {
        private readonly RocketMqConfig _config;

        public DefaultRocketMqConnection(RocketMqConfig config = null)
        {
            _config = config ?? RocketMqConfig.Config();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string Broker { get; }
        public bool IsConnected { get; }
        public bool TryConnect()
        {
            throw new NotImplementedException();
        }
    }
}
