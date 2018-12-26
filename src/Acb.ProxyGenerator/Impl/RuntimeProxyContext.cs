using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.ProxyGenerator.Impl
{
    internal sealed class RuntimeProxyContext : ProxyContext, IDisposable
    {
        private volatile IDictionary<string, object> _data;
        private readonly IServiceProvider _serviceProvider;
        private readonly MethodInfo _implementationMethod;
        private readonly object _implementation;
        private bool _disposedValue = false;

        public override IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    throw new NotSupportedException("The current context does not support IServiceProvider.");
                }

                return _serviceProvider;
            }
        }

        public override IDictionary<string, object> AdditionalData
        {

            get
            {
                if (_data == null)
                {
                    _data = new Dictionary<string, object>();
                }
                return _data;
            }
        }

        public override object ReturnValue { get; set; }

        public override MethodInfo ServiceMethod { get; }

        public override object[] Parameters { get; }

        public override MethodInfo ProxyMethod { get; }

        public override object Proxy { get; }

        public override MethodInfo ImplementationMethod => _implementationMethod;

        public override object Implementation => _implementation;

        public RuntimeProxyContext(
            IServiceProvider serviceProvider, MethodInfo serviceMethod, MethodInfo targetMethod, MethodInfo proxyMethod,
            object targetInstance, object proxyInstance, object[] parameters)
        {
            _serviceProvider = serviceProvider;
            _implementationMethod = targetMethod;
            _implementation = targetInstance;
            ServiceMethod = serviceMethod;
            ProxyMethod = proxyMethod;
            Proxy = proxyInstance;
            Parameters = parameters;
        }

        public override async Task Complete()
        {
            if (_implementation == null || _implementationMethod == null)
            {
                await Break();
                return;
            }
            var reflector = ProxyContextExtensions.ReflectorTable.GetOrAdd(_implementationMethod, method => method.GetReflector(CallOptions.Call));
            var returnValue = reflector.Invoke(_implementation, Parameters);
            await this.AwaitIfAsync(returnValue);
            ReturnValue = returnValue;
        }

        public override Task Break()
        {
            if (ReturnValue == null)
            {
                ReturnValue = ServiceMethod.ReturnParameter.ParameterType.GetDefaultValue();
            }
            return Task.FromResult(false);
        }

        public override Task Invoke(ProxyDelegate next)
        {
            return next(this);
        }

        public void Dispose()
        {
            if (_disposedValue)
            {
                return;
            }

            if (_data == null)
            {
                _disposedValue = true;
                return;
            }

            foreach (var key in _data.Keys.ToArray())
            {
                _data.TryGetValue(key, out object value);

                var disposable = value as IDisposable;

                disposable?.Dispose();

                _data.Remove(key);
            }

            _disposedValue = true;
        }
    }
}