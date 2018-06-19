using Acb.Core.Logging;
using Acb.Framework;
using Acb.Spear.DotNetty;
using Acb.Spear.Message;
using Acb.Spear.Micro;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogManager.LogLevel(LogLevel.All);
            var bootstrap = DBootstrap.Instance;
            bootstrap.BuilderHandler += builder =>
            {
            };
            bootstrap.Initialize();
            var listener = new DotNettyMicroListener(new JsonMessageCoderFactory(),
                new MicroExecutor(new MicroEntryFactory()));
            using (listener)
            {
                var point = new IPEndPoint(IPAddress.Any, 98);
                listener.Received += ListenerReceived;
                listener.Start(point).Wait();
                while (true)
                {
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "exit":
                            listener.Close();
                            return;
                    }
                }
            }
        }

        private static async Task ListenerReceived(IMicroSender sender, MicroMessage message)
        {
            var inv = message.GetContent<InvokeMessage>();
            var result = new ResultMessage(inv).Create(message.Id);
            await sender.Send(result);
        }
    }
}
