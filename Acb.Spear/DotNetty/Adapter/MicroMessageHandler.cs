﻿using Acb.Spear.Message;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Acb.Spear.DotNetty.Adapter
{
    public class MicroMessageHandler : ChannelHandlerAdapter
    {
        private readonly IMessageDecoder _messageDecoder;

        public MicroMessageHandler(IMessageDecoder messageDecoder)
        {
            _messageDecoder = messageDecoder;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);
            var transportMessage = _messageDecoder.Decode(data);
            context.FireChannelRead(transportMessage);
            ReferenceCountUtil.Release(buffer);
        }
    }
}
