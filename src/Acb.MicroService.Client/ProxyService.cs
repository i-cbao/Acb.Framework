using Acb.Core;
using System.Reflection;

namespace Acb.MicroService.Client
{
    public class ProxyService
    {
        /// <summary> 生成代理 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Proxy<T>() where T : IMicroService
        {
            return DispatchProxy.Create<T, InvokeProxy<T>>();
        }
    }
}
