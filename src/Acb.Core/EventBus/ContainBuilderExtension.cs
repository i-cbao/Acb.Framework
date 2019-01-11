using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Core.EventBus
{
    public static class ContainBuilderExtension
    {
        /// <summary> 开启订阅 </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider SubscribeAt(this IServiceProvider provider)
        {
            provider.GetService<ISubscribeAdapter>().SubscribeAt();
            return provider;
        }
    }
}
