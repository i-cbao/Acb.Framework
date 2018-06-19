using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Spear.DotNetty;
using Acb.Spear.Message;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = LogManager.Logger<Program>();
            LogManager.LogLevel(LogLevel.All);
            var factory = new DotNettyClientFactory(new JsonMessageCoderFactory());
            var point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 98);
            //var point = new DnsEndPoint("127.0.0.1", 98);
            while (true)
            {
                var message = Console.ReadLine();
                try
                {
                    Task.Run(async () =>
                    {
                        var client = factory.CreateClient(point);
                        var inv = new InvokeMessage
                        {
                            ServiceId = message,
                            Parameters = new Dictionary<string, object>
                            {
                                {"id", RandomHelper.Random().Next(1000, 9999)}
                            }
                        };
                        var result = await client.Send(inv);
                        logger.Info(result);
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
            }
        }
    }
}
