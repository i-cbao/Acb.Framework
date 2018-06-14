using System.Collections.Generic;
using System.Reflection;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal interface IServiceFinder
    {
        List<string> Find(Assembly ass, MicroServiceConfig config);
    }
}
