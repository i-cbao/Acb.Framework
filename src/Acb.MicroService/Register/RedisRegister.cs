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

        private readonly MicroServiceConfig _config;
        private HashSet<Assembly> _asses;
        private readonly IDatabase _redis;

        public RedisRegister(MicroServiceConfig config)
        {
            _redis = CurrentIocManager.Resolve<RedisManager>().GetDatabase();
            _config = config;
        }

        private string ServiceUrl()
        {
            return _config == null ? string.Empty : $"http://{_config.Host}:{_config.Port}/";
        }

        private static string AssKey(Assembly ass)
        {
            return $"{RegistCenterKey}:{Consts.Mode}:{ass.GetName().Name}";
        }

        public void Regist(HashSet<Assembly> asses)
        {
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
