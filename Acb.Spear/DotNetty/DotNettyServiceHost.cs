using Acb.Core.Logging;
using Acb.Spear.DotNetty.Adapter;
using Acb.Spear.Message;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.DotNetty
{
    public class DotNettyServiceHost : IServerHost
    {
        private IChannel _channel;
        private readonly ILogger _logger;
        private readonly IMessageCoderFactory _coderFactory;

        public DotNettyServiceHost()
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            _logger = LogManager.Logger<DotNettyServiceHost>();
            _coderFactory = new JsonMessageCoderFactory();
        }

        public async Task Start(EndPoint endPoint)
        {
            _logger.Debug($"准备启动服务主机，监听地址：{endPoint}。");
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();//Default eventLoopCount is Environment.ProcessorCount * 2
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildOption(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new MicroMessageHandler(_coderFactory.GetDecoder()));
                    pipeline.AddLast(new ServerHandler());
                    //pipeline.AddLast(new TransportMessageChannelHandlerAdapter(transportMessageDecoder));
                    //pipeline.AddLast(new ServerHandler(async (contenxt, message) =>
                    //{
                    //    var sender = new DotNettyServerMessageSender(_messageEncoder, contenxt);
                    //    await OnReceived(sender, message);
                    //}));
                }));
            _channel = await bootstrap.BindAsync(endPoint);
            _logger.Debug($"服务主机启动成功，监听地址：{endPoint}。");
        }

        public Task Close()
        {
            return Task.Run(async () =>
            {
                await _channel.EventLoop.ShutdownGracefullyAsync();
                await _channel.CloseAsync();
            });
        }

        public void Dispose()
        {
            if (_channel == null)
                return;
            Task.Run(async () =>
            {
                await _channel.DisconnectAsync();
            }).Wait();
        }
    }
}
