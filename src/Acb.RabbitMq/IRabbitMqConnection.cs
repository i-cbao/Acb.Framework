using Acb.Core.Dependency;
using RabbitMQ.Client;
using System;

namespace Acb.RabbitMq
{
    public interface IRabbitMqConnection : IDisposable, IScopedDependency
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
