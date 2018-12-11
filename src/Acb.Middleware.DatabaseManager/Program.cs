using Acb.WebApi;

namespace Acb.Middleware.DatabaseManager
{
    public class Program : DHost<Startup>
    {
        public static void Main(string[] args)
        {
            Start(args);
        }
    }
}
