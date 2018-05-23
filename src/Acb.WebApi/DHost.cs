using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Acb.WebApi
{
    public class DHost : DHost<DStartup>
    {
    }

    public class DHost<TSTart> where TSTart : DStartup
    {
        protected static event Action<IWebHostBuilder> Builder;

        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static void Start(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<TSTart>();
            Builder?.Invoke(builder);
            using (var host = builder.Build())
            {
                host.Run();
            }
        }
    }
}
