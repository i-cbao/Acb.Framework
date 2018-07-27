using Acb.Core;
using Acb.Core.Extensions;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.Proxy
{
    /// <summary> Netty代理 </summary>
    /// <typeparam name="T"></typeparam>
    public class NettyProxy<T> : ProxyAsync where T : IMicroService
    {
        private async Task<object> BaseInvoke(MethodInfo method, object[] args)
        {
            return await Task.FromResult(1);
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            return BaseInvoke(method, args).GetAwaiter().GetResult();
        }

        public override Task InvokeAsync(MethodInfo method, object[] args)
        {
            return BaseInvoke(method, args);
        }

        public override async Task<TR> InvokeAsyncT<TR>(MethodInfo method, object[] args)
        {
            var result = await BaseInvoke(method, args);
            return result.CastTo<TR>();
        }
    }
}
