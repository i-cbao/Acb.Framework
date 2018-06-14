using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Acb.MicroService
{
    /// <summary> 微服务主机 </summary>
    public class MicroServiceHost
    {
        protected static event Action<IWebHostBuilder> Builder;
        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static void Start(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables("ASPNETCORE_")
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel(opt => opt.ConfigureEndpoints())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<MicroServiceStartup>();
            Builder?.Invoke(builder);
            using (var host = builder.Build())
            {
                host.Run();
            }
        }
    }
}
