using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal interface IServiceFinder
    {
        Task<List<string>> Find(Assembly ass, MicroServiceConfig config);
    }
}
