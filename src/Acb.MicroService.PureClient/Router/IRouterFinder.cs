using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.MicroService.PureClient.Router
{
    public interface IRouterFinder
    {
        /// <summary> 服务发现 </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        Task<List<ServiceAddress>> Find(Type serviceType);
    }
}
