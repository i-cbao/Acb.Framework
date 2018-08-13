using Acb.Core.Dependency;
using System;

namespace Acb.RocketMq
{
    public interface IRocketMqConnection : IDisposable, IScopedDependency
    {
        string Broker { get; }
        bool IsConnected { get; }

        bool TryConnect();
    }
}
