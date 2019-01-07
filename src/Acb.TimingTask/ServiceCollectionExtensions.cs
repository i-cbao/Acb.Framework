using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.TimingTask
{
    /// <summary> 服务扩展 </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary> 添加定时任务 </summary>
        /// <param name="services"></param>
        /// <param name="triggers">
        /// 定时器格式：{name,corn}
        /// corn:corn表达式 Or {seconds,repeat?}
        /// repeat:-1 or number
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddTimingTask(this IServiceCollection services, IDictionary<string, string> triggers = null)
        {
            services.TryAddSingleton(provider => new SchedulerCenter(provider, triggers));
            return services;
        }

        /// <summary> 使用定时任务 </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider UseTimingTask(this IServiceProvider provider)
        {
            var center = provider.GetService<SchedulerCenter>();
            Task.Run(async () => { await center.StartScheduler(); });
            return provider;
        }
    }
}
