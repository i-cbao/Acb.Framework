using System.Threading.Tasks;
using Acb.WebApi;

namespace Acb.FileServer
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
