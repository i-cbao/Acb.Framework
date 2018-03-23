using Acb.Framework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Acb.MicroService
{
    public class MicroServiceHost
    {
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
            MicroServiceRegister.UnRegist();
        }
    }
}
