using Acb.MicroService;

namespace Acb.Demo.MicroService
{
    public class Program : MicroServiceHost<DemoStartup>
    {
        public static void Main(string[] args)
        {
            Start(args);
        }
    }
}
