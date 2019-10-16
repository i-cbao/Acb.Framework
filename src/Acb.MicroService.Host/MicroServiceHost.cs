using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Acb.MicroService.Host
{
    public class MicroServiceHost : MicroServiceHost<MicroServiceStartup>
    {
    }

    /// <summary> 微服务主机 </summary>
    public class MicroServiceHost<TStartup> where TStartup : MicroServiceStartup
    {
        protected static event Action<IHostBuilder> Builder;
        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static async Task Start(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables("ASPNETCORE_")
                .Build();

            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options => { options.ConfigureEndpoints(); })
                        .UseIISIntegration()
                        .UseStartup<TStartup>();
                });
            Builder?.Invoke(builder);
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
