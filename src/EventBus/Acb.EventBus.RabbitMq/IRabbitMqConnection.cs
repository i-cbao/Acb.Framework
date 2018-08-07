using RabbitMQ.Client;
using System;

namespace Acb.EventBus.RabbitMq
{
    public interface IRabbitMqConnection : IDisposable
    {
        string Broker { get; }
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
