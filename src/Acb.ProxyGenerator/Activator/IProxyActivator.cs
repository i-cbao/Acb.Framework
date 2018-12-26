using System.Threading.Tasks;

namespace Acb.ProxyGenerator.Activator
{
    public interface IProxyActivator
    {
        TResult Invoke<TResult>(ProxyActivatorContext activatorContext);

        Task<TResult> InvokeTask<TResult>(ProxyActivatorContext activatorContext);

        ValueTask<TResult> InvokeValueTask<TResult>(ProxyActivatorContext activatorContext);
    }
}