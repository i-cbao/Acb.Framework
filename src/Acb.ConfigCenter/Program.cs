using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Acb.ConfigCenter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseKestrel().UseIISIntegration().UseStartup<Startup>();
                })
                .Build();
            await host.RunAsync();
        }
    }
}
