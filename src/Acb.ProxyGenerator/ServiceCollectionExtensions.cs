using Acb.ProxyGenerator.Activator;
using Acb.ProxyGenerator.Impl;
using Acb.ProxyGenerator.Validator;
using Acb.ProxyGenerator.Validator.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Acb.ProxyGenerator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            services.AddSingleton<IProxyValidatorBuilder, ProxyValidatorBuilder>();
            services.AddSingleton<IProxyTypeGenerator, ProxyTypeGenerator>();
            services.AddSingleton<IProxyContextFactory, ProxyContextFactory>();
            services.AddSingleton<IProxyBuilderFactory, ProxyBuilderFactory>();
            services.AddSingleton<IProxyActivatorFactory, ProxyActivatorFactory>();
            services.AddSingleton<IProxyGenerator, Impl.ProxyGenerator>();
            return services;
        }

        public static object FastInvoke(this MethodInfo method, object instance, params object[] parameters)
        {
            var fastInvoke = ProxyGenerator.FastInvoke.GetMethodInvoker(method);
            return fastInvoke(instance, parameters);
            //return method.GetReflector().Invoke(instance, parameters);
        }
    }
}
