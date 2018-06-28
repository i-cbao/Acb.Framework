using Acb.Core;
using Acb.Core.Dependency;
using Acb.Redis;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Register
{
    internal class RedisRegister : IRegister
    {
        private const string RegistCenterKey = MicroServiceRegister.MicroSreviceKey + ":center";

        private MicroServiceConfig _config;
        private HashSet<Assembly> _asses;
        private readonly IDatabase _redis;

        public RedisRegister()
        {
            _redis = CurrentIocManager.Resolve<RedisManager>().GetDatabase();
        }

        private string ServiceUrl()
        {
            return _config == null ? string.Empty : $"http://{_config.Host}:{_config.Port}/";
        }

        private static string AssKey(Assembly ass)
        {
            return $"{RegistCenterKey}:{Consts.Mode}:{ass.GetName().Name}";
        }

        public void Regist(HashSet<Assembly> asses, MicroServiceConfig config)
        {
            _config = config;
            _asses = asses;
            foreach (var ass in asses)
            {
                _redis.SetAdd(AssKey(ass), ServiceUrl());
            }
        }

        public void Deregist()
        {
            if (_asses == null || !_asses.Any())
                return;
            foreach (var ass in _asses)
            {
                _redis.SetRemove(AssKey(ass), ServiceUrl());
            }
        }
    }
}
