using Acb.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.Proxy
{
    /// <summary> Netty代理 </summary>
    /// <typeparam name="T"></typeparam>
    public class NettyProxy<T> : ProxyAsync where T : IMicroService
    {
        protected override async Task<object> BasicInvokeAsync(MethodInfo method, object[] args)
        {
            return await Task.FromResult(1);
        }
    }
}
