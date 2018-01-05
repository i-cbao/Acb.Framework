using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Modules;

namespace Acb.Redis
{
    [DependsOn(typeof(CoreModule))]
    public class RedisModule : DModule
    {
        public override void Initialize()
        {
            CacheManager.SetProvider(CacheLevel.Second, new RedisCacheProvider());
            base.Initialize();
        }

        public override void Shutdown()
        {
            RedisManager.Instance.Dispose();
            base.Shutdown();
        }
    }
}
