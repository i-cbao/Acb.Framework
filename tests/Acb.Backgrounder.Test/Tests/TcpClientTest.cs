using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Text;

namespace Acb.Backgrounder.Test.Tests
{
    public class TcpClientTest : ConsoleTest
    {
        private TcpClient _client;
        private NetworkStream _stream;
        public override void OnMapServiceCollection(IServiceCollection services)
        {
            _client = new TcpClient("127.0.0.1", 9999);
            _stream = _client.GetStream();
            base.OnMapServiceCollection(services);
        }

        public override void OnCommand(string cmd, IContainer provider)
        {
            var data = Encoding.UTF8.GetBytes(cmd);
            _stream.Write(data, 0, data.Length);
            Logger.Info($"send msg:{cmd}");

            base.OnCommand(cmd, provider);
        }

        public override void OnShutdown()
        {
            _client?.Dispose();
            _stream?.Dispose();
            base.OnShutdown();
        }
    }
}
