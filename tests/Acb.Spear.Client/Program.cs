using Acb.Core.Logging;
using Acb.Spear.DotNetty;
using Acb.Spear.Message;
using System;
using System.Net;

namespace Acb.Spear.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogManager.LogLevel(LogLevel.All);
            var factory = new DotNettyClientFactory();
            var server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 98);
            while (true)
            {
                var message = Console.ReadLine();
                try
                {
                    var client = factory.CreateClient(server);
                    client.Send(new InvokeMessage
                    {
                        ServiceId = message
                    }.Create()).Wait();
                }
                catch (Exception ex)
                {
                    LogManager.Logger<Program>().Error(ex.Message, ex);
                }
            }
        }
    }
}
