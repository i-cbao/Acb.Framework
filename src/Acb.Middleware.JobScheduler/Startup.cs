using Acb.Middleware.JobScheduler.Scheduler;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Middleware.JobScheduler
{
    /// <summary> 启动类 </summary>
    public class Startup : DStartup
    {
        /// <summary> 构造函数 </summary>
        public Startup() : base("任务调度中心文档") { }

        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
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
            base.Configure(app, env);
        }
    }
}
