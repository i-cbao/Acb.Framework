using Acb.Core.Extensions;
using Acb.Spear.Contracts;
using Acb.Spear.Hubs;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Acb.Spear
{
    /// <summary> 启动类 </summary>
    public class Startup : DStartup
    {
        /// <summary> 构造函数 </summary>
        public Startup() : base("分布式管理中心接口文档") { }

        protected override void UseServices(IServiceProvider provider)
        {
            Task.Run(async () => { await provider.GetService<ISchedulerContract>().Start(); });
            base.UseServices(provider);
        }

        protected override void MapServices(IServiceCollection services)
        {
            base.MapServices(services);
        }

        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
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
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = new[] { "index.html" } }
            });
            app.UseSignalR(route => { route.MapHub<ConfigHub>("/config_hub"); });
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            base.Configure(app, env);
        }
    }
}
