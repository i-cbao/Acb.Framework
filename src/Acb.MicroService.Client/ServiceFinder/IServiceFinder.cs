using System.Reflection;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal interface IServiceFinder
    {
        string[] Find(Assembly ass, MicroServiceConfig config);
    }
}
