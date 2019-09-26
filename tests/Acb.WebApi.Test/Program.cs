using System.Threading.Tasks;

namespace Acb.WebApi.Test
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
