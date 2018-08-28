using Acb.Redis;
using Acb.Spear.Hubs;
using Acb.Spear.Scheduler;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Spear
{
    /// <summary> 启动类 </summary>
    public class Startup : DStartup
    {
        /// <summary> 构造函数 </summary>
        public Startup() : base("分布式管理中心接口文档") { }

        protected override void UseServices(IServiceProvider provider)
        {
            provider.GetService<SchedulerCenter>().StartScheduler().GetAwaiter().GetResult();
            base.UseServices(provider);
        }

        protected override void MapServices(IServiceCollection services)
        {
            services.AddRedisEventBus();
            base.MapServices(services);
        }

        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.TryAddSingleton<SchedulerCenter>();
            return base.ConfigureServices(services);
        }

        /// <summary> 配置 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseSignalR(route => { route.MapHub<ConfigHub>("/config_hub"); });
            base.Configure(app, env);
        }
    }
}
