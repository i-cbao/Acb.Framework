using Acb.Core;
using Acb.Redis;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            _redis = RedisManager.Instance.GetDatabase();
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

        public async Task Regist(HashSet<Assembly> asses)
        {
            _asses = asses;
            foreach (var ass in asses)
            {
                await _redis.SetAddAsync(AssKey(ass), ServiceUrl());
            }
        }

        public async Task Deregist()
        {
            if (_asses == null || !_asses.Any())
                return;
            foreach (var ass in _asses)
            {
                await _redis.SetRemoveAsync(AssKey(ass), ServiceUrl());
            }
        }
    }
}
