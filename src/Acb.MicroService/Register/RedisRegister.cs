using Acb.Core;
using Acb.Core.Extensions;
using Acb.Redis;
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

        private string ServiceUrl()
        {
            return _config == null ? string.Empty : $"http://{_config.Host}:{_config.Port}/";
        }

        public void Regist(HashSet<Assembly> asses, MicroServiceConfig config)
        {
            _config = config;
            _asses = asses;
            var redis = RedisManager.Instance.GetDatabase();
            foreach (var ass in asses)
            {
                redis.SetAdd($"{RegistCenterKey}:{Consts.Mode}:{ass.AssemblyKey()}", ServiceUrl());
            }
        }

        public void Deregist()
        {
            if (_asses == null || !_asses.Any())
                return;
            var redis = RedisManager.Instance.GetDatabase();
            foreach (var type in _asses)
            {
                redis.SetRemove($"{RegistCenterKey}:{Consts.Mode}:{type}", ServiceUrl());
            }
        }
    }
}
