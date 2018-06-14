using Acb.Spear.Message;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace Acb.Spear.DotNetty
{
    public class DotNettyMicroClient : IMicroClient
    {
        private readonly IChannel _channel;
        private readonly IMessageEncoder _messageEncoder;


        public DotNettyMicroClient(IChannel channel, IMessageEncoder messageEncoder)
        {
            _channel = channel;
            _messageEncoder = messageEncoder;
        }

        public async Task Send(IMicroMessage message, bool flush = true)
        {
            var data = _messageEncoder.Encode(message);
            var buffer = Unpooled.WrappedBuffer(data);
            if (flush)
                await _channel.WriteAndFlushAsync(buffer);
            else
                await _channel.WriteAsync(buffer);
        }

        public T CreateProxy<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}
