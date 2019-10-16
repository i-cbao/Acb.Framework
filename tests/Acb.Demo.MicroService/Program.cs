using Acb.MicroService.Host;
using System.Threading.Tasks;

namespace Acb.Demo.MicroService
{
    public class Program : MicroServiceHost<DemoStartup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
