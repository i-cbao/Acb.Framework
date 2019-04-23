using Acb.Core.EventBus.Options;
using System;
using System.Threading.Tasks;
using Acb.Core.Message;

namespace Acb.Core.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        /// <summary> 订阅管理器 </summary>
        protected readonly ISubscribeManager SubscriptionManager;

        /// <summary> 编解码器 </summary>
        public IMessageCodec Codec { get; }

        protected AbstractEventBus(ISubscribeManager manager, IMessageCodec messageCodec)
        {
            SubscriptionManager = manager ?? new DefaultSubscribeManager();
            Codec = messageCodec;
        }

        public abstract Task Subscribe<T, TH>(Func<TH> handler, SubscribeOption option = null)
            where TH : IEventHandler<T>;

        public Task Unsubscribe<T, TH>() where TH : IEventHandler<T>
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
            return Task.CompletedTask;
        }

        public Task Publish(string key, object message, PublishOption option = null)
        {
            var data = Codec.Encode(message);
            return Publish(key, data, option);
        }

        public abstract Task Publish(string key, byte[] message, PublishOption option = null);
    }
}
