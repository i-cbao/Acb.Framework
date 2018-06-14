using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Acb.ConfigCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}
