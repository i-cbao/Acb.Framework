using System.Threading.Tasks;

namespace Acb.ProxyGenerator.Attributes
{
    public abstract class AopAttribute
    {
        public abstract Task Invoke(ProxyContext context, ProxyDelegate next);
    }
}
