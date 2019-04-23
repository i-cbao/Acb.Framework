using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Message;
using Acb.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Router
{
    public class RedisRouter : IServiceRouter
    {
        private readonly IDictionary<string, RedisValue> _services;
        private readonly MicroServiceConfig _serviceConfig;
        private readonly IMessageCodec _messageCodec;
        private readonly IDatabase _redis;

        public RedisRouter(IMessageCodec messageCodec, MicroServiceConfig serviceConfig)
        {
            _serviceConfig = serviceConfig;
            _messageCodec = messageCodec;
            _services = new Dictionary<string, RedisValue>();
            _redis = RedisManager.Instance.GetDatabase();
        }

        private static string AssKey(Assembly ass)
        {
            return $"{Constants.RegistCenterKey}:{Consts.Mode}:{ass.ServiceName()}";
        }

        public async Task Regist(IEnumerable<Assembly> serviceAssemblies, ServiceAddress address)
        {
            foreach (var ass in serviceAssemblies)
            {
                var name = AssKey(ass);
                var router = _messageCodec.Encode(address);
                await _redis.SetAddAsync(name, router);
                _services.Add(name, router);
            }
        }

        public async Task<List<ServiceAddress>> Find(Type serviceType)
        {
            var urlList = new List<ServiceAddress>();
            var ass = serviceType.Assembly;
            var list = await _redis.SetMembersAsync($"{Constants.RegistCenterKey}:{Consts.Mode}:{ass.GetName().Name}");
            urlList.AddRange(list.Select(t => _messageCodec.Decode<ServiceAddress>(t)));
            if (Consts.Mode == ProductMode.Dev)
            {
                list = await _redis.SetMembersAsync(
                    $"{Constants.RegistCenterKey}:{ProductMode.Test}:{ass.GetName().Name}");
                urlList.AddRange(list.Select(t => _messageCodec.Decode<ServiceAddress>(t)));
            }

            return urlList;
        }

        public async Task Deregist()
        {
            if (_services == null || !_services.Any())
                return;
            foreach (var service in _services)
            {
                await _redis.SetRemoveAsync(service.Key, service.Value);
            }
        }
    }
}
