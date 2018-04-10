using System.Reflection;

namespace Acb.MicroService.Client.ServiceFind
{
    internal interface IServiceFinder
    {
        string[] Find(Assembly ass, MicroServiceConfig config);
    }
}
