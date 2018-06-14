using Acb.Core.Logging;
using DotNetty.Transport.Channels;
using System;

namespace Acb.Spear.DotNetty.Adapter
{
    /// <summary> 客户端处理器 </summary>
    public class ClientHandler : ChannelHandlerAdapter
    {
        private readonly Action<IChannel> _removeAction;
        private readonly ILogger _logger;

        public ClientHandler(Action<IChannel> removeAction)
        {
            _removeAction = removeAction;
            _logger = LogManager.Logger<ClientHandler>();
        }
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _removeAction(context.Channel);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            _logger.Info(message);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.Error($"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。", exception);
        }
    }
}
