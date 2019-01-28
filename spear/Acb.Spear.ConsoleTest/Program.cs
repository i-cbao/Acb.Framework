using Asb.Spear.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using Acb.Core.Data.Config;
using Acb.Core.Extensions;
using Acb.Framework;

namespace Acb.Spear.ConsoleTest
{
    public class Program : ConsoleHost
    {
        private static void Main(string[] args)
        {
            MapServiceCollection += services =>
            {
                services.AddSpear(new SpearOption
                {
                    Host = "localhost",
                    Port = 53454,
                    Code = "ichebao",
                    Secret = "123456"
                });
            };

            UseServiceProvider += provider =>
            {
                provider.UseSpear(new[] { "basic", "dapper" });
            };
            Command += (cmd, container) =>
            {
                if (cmd.StartsWith("dapper:"))
                {
                    var cnf = cmd.Config<ConnectionConfig>();
                    Console.WriteLine(JsonConvert.SerializeObject(cnf));
                }
                else
                {
                    Console.WriteLine(cmd.Config<string>());
                }
            };

            Start(args);

            var client = Resolve<SpearClient>();
            client.ConfigChange += config =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
            };
        }
    }
}
