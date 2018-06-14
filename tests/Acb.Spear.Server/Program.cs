using Acb.Core.Logging;
using Acb.Spear.DotNetty;
using System;
using System.Net;

namespace Acb.Spear.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.LogLevel(LogLevel.All);
            var host = new DotNettyServiceHost();
            using (host)
            {
                host.Start(new IPEndPoint(IPAddress.Any, 98)).Wait();
                while (true)
                {
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "exit":
                            host.Close();
                            return;
                    }
                }
                //Task.Run(async () => { await host.Start(new IPEndPoint(IPAddress.Any, 98)); });
            }
        }
    }
}
