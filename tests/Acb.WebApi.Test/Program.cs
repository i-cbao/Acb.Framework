using Acb.Framework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Acb.WebApi.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            DBootstrap.Instance.Dispose();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
