using Acb.WebApi;

namespace Acb.FileServer
{
    public class Program : DHost<Startup>
    {
        public static void Main(string[] args)
        {
            Start(args);
        }
    }
}
