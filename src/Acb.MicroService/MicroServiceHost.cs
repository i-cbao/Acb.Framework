using Acb.Framework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Acb.MicroService
{
    /// <summary> 微服务主机 </summary>
    public class MicroServiceHost
    {
        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static void Start(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<MicroServiceStartup>()
                .Build();
            using (host)
            {
                host.Run();
            }
            DBootstrap.Instance.Dispose();
            MicroServiceRegister.Deregist();
        }
    }
}
