using System;
using System.Threading.Tasks;

namespace Acb.MicroService.PureClient.Router
{
    public interface IRouter : IRouterRegister, IRouterFinder
    {
        /// <summary> 清空服务缓存 </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        Task CleanCache(Type serviceType);
    }
}
