using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Acb.WebApi
{
    public class DHost : DHost<DStartup>
    {
    }

    public class DHost<TStart> where TStart : DStartup
    {
        protected static event Action<IHostBuilder> Builder;

        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static async Task Start(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options =>
                        {
                        })
                        .UseIISIntegration()
                        .UseStartup<TStart>();
                });

            Builder?.Invoke(builder);
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
