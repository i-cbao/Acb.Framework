using Acb.Core.Logging;
using Acb.Spear.DotNetty.Adapter;
using Acb.Spear.Message;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Concurrent;
using System.Net;

namespace Acb.Spear.DotNetty
{
    public class DotNettyClientFactory
    {
        private readonly Bootstrap _bootstrap;
        private readonly ILogger _logger;
        private readonly IMessageCoderFactory _coderFactory;

        private readonly ConcurrentDictionary<EndPoint, Lazy<IMicroClient>> _clients =
            new ConcurrentDictionary<EndPoint, Lazy<IMicroClient>>();

        private static readonly AttributeKey<EndPoint> OrigEndPointKey =
            AttributeKey<EndPoint>.ValueOf(typeof(DotNettyClientFactory), nameof(EndPoint));

        public DotNettyClientFactory()
        {
            _coderFactory = new JsonMessageCoderFactory();
            LogManager.AddAdapter(new ConsoleAdapter());
            _logger = LogManager.Logger<DotNettyClientFactory>();
            _bootstrap = GetBootstrap();
            _bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            {
                var pipeline = c.Pipeline;
                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast(new MicroMessageHandler(_coderFactory.GetDecoder()));
                pipeline.AddLast(new ClientHandler(channel =>
                {
                    var k = channel.GetAttribute(OrigEndPointKey).Get();
                    _logger.Debug($"删除客户端：{k}");
                    _clients.TryRemove(k, out _);
                }));
            }));
        }

        private static Bootstrap GetBootstrap()
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .Group(new MultithreadEventLoopGroup(1));

            return bootstrap;
        }

        /// <summary> 创建客户端 </summary>
        /// <param name="endPoint">终结点。</param>
        /// <returns>传输客户端实例。</returns>
        public IMicroClient CreateClient(EndPoint endPoint)
        {
            var key = endPoint;
            try
            {
                return _clients.GetOrAdd(key, k => new Lazy<IMicroClient>(() =>
                    {
                        _logger.Debug($"准备为服务端地址：{key}创建客户端。");
                        var bootstrap = _bootstrap;
                        var channel = bootstrap.ConnectAsync(k).Result;
                        channel.GetAttribute(OrigEndPointKey).Set(k);
                        return new DotNettyMicroClient(channel, _coderFactory.GetEncoder());
                    }
                )).Value;
            }
            catch (Exception ex)
            {
                _clients.TryRemove(key, out _);
                throw;
            }
        }
    }
}
