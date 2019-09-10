using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test.Tests
{
    public class TcpSocketTest : ConsoleTest
    {
        private TcpListener _listener;
        //private IDictionary<string, TcpClient> _clients;
        private void StartListen(int port)
        {
            //_clients = new Dictionary<string, TcpClient>();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Info($"start listen at {port}");
            Logger.Warn($"start listen at {port}");
            Logger.Error($"start listen at {port}");
            Logger.Fatal($"start listen at {port}");
            _listener.BeginAcceptTcpClient(AcceptClient, _listener);
        }

        protected void AcceptClient(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);
            Logger.Info($"client [{client.Client.RemoteEndPoint}] connected");
            listener.BeginAcceptTcpClient(AcceptClient, listener);
            var stream = client.GetStream();
            while (true)
            {
                if (!client.Connected)
                {
                    Logger.Info($"client [{client.Client.RemoteEndPoint}] disconnect");
                    client.Close();
                    stream.Close();
                    break;
                }

                var buffer = new byte[1024];
                stream.BeginRead(buffer, 0, buffer.Length, Receive, new object[] { client, stream, buffer });
            }
        }

        protected void Receive(IAsyncResult ar)
        {
            var param = ar.AsyncState as object[];
            if (param == null || param.Length != 3)
                return;
            var client = param[0] as TcpClient;
            var stream = param[1] as NetworkStream;
            var buffer = param[2] as byte[];
            if (client == null || buffer == null || stream == null)
                return;
            stream?.EndRead(ar);
            var msg = Encoding.UTF8.GetString(buffer);
            Logger.Info($"{client.Client.RemoteEndPoint} : {msg}");
            if (msg == "exit")
            {
                Logger.Info($"client [{client.Client.RemoteEndPoint}] disconnect");
            }
            stream?.BeginRead(buffer, 0, buffer.Length, Receive, param);

        }

        public override void OnShutdown()
        {
            _listener?.Stop();
            base.OnShutdown();
        }

        public override void OnMapServiceCollection(IServiceCollection services)
        {
            base.OnMapServiceCollection(services);
        }

        public override void OnUseServiceProvider(IServiceProvider provider)
        {
            Task.Run(() => StartListen(9999));
            base.OnUseServiceProvider(provider);
        }

        public override void OnCommand(string cmd, IContainer provider)
        {
            base.OnCommand(cmd, provider);
        }
    }
}
