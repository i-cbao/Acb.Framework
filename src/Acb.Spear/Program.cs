using Acb.WebApi;

namespace Acb.Middleware.JobScheduler
{
    public class Program : DHost<Startup>
    {
        public static void Main(string[] args)
        {
            Start(args);
        }
    }
}
