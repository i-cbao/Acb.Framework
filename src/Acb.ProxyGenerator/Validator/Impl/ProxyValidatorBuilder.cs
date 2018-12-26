using System;
using System.Collections.Generic;

namespace Acb.ProxyGenerator.Validator.Impl
{
    public sealed class ProxyValidatorBuilder : IProxyValidatorBuilder
    {
        private readonly IList<Func<ProxyValidationDelegate, ProxyValidationDelegate>> _collections;

        public ProxyValidatorBuilder()
        {
            _collections = new List<Func<ProxyValidationDelegate, ProxyValidationDelegate>>();

            //foreach (var handler in configuration.ValidationHandlers.OrderBy(x => x.Order))
            //{
            //    _collections.Add(next => method => handler.Invoke(method, next));
            //}
        }

        public IProxyValidator Build()
        {
            ProxyValidationDelegate invoke = method => false;

            var count = _collections.Count;

            for (var i = count - 1; i > -1; i--)
            {
                invoke = _collections[i](invoke);
            }

            return new ProxyValidator(invoke);
        }
    }
}
