using Acb.Spear.Message;
using Acb.Spear.Transport;
using Acb.Spear.Transport.Impl;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.Runtime.Server.Impl
{
    public abstract class ServiceHost : IServiceHost
    {
        private readonly IServiceExecutor _serviceExecutor;

        /// <summary> 消息监听者 </summary>
        protected IMessageListener MessageListener { get; } = new MessageListener();

        protected ServiceHost(IServiceExecutor serviceExecutor)
        {
            _serviceExecutor = serviceExecutor;
            MessageListener.Received += MessageListener_Received;
        }

        private async Task MessageListener_Received(IMessageSender sender, TransportMessage message)
        {
            await _serviceExecutor.ExecuteAsync(sender, message);
        }

        public abstract void Dispose();

        public abstract Task StartAsync(EndPoint endPoint);
    }
}
