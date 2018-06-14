using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Modules;
using System.Threading;

namespace Acb.Redis
{
    [DependsOn(typeof(CoreModule))]
    public class RedisModule : DModule
    {
        public override void Initialize()
        {
            //设置线程数，防止并发超时
            ThreadPool.GetMinThreads(out _, out var compt);
            ThreadPool.SetMinThreads(300, compt);
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
