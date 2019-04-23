using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService
{
    /// <summary> 服务路由器(注册与发现) </summary>
    public interface IServiceRouter
    {
        /// <summary> 服务注册 </summary>
        /// <param name="serviceAssemblies"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task Regist(IEnumerable<Assembly> serviceAssemblies, ServiceAddress address);

        /// <summary> 服务发现 </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        Task<List<ServiceAddress>> Find(Type serviceType);

        Task CleanCache(Type serviceType);

        /// <summary> 注销服务 </summary>
        /// <returns></returns>
        Task Deregist();
    }
}
