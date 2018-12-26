using System.Reflection;

namespace Acb.ProxyGenerator
{
    public interface IProxyValidator
    {
        bool Validate(MethodInfo method, bool isStrictValidation);
    }
}