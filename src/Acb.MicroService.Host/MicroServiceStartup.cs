using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Extensions;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.MicroService.Host.Filters;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Acb.MicroService.Host
{
    public class MicroServiceStartup
    {
        private readonly DBootstrap _bootstrap;

        public MicroServiceStartup()
        {
            _bootstrap = new DBootstrap();
        }

        protected virtual void MapServices(ContainerBuilder builder)
        {

        }

        protected virtual void MapServices(IServiceCollection services)
        {

        }

        protected virtual void UseServices(IServiceProvider provider)
        {
        }

        /// <summary> 配置服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSystemLogging();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options =>
            {
                //自定义异常捕获
                options.Filters.Add<DExceptionFilter>();
            }).AddControllersAsServices();

            services.AddHealthChecks();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<MicroServiceRegister>();
            services.AddSingleton<MicroServiceRunner>();

            //使用Json编解码
            services.AddJsonCodec();

            //使用服务注册发现
            services.AddMicroRouter();

            MapServices(services);
        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            _bootstrap.Builder = builder;
            _bootstrap.BuilderHandler += MapServices;
            _bootstrap.Initialize();
        }

        protected virtual void ConfigRoute(IEndpointRouteBuilder builder)
        {

        }

        /// <summary> 配置应用 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="applicationLifetime"></param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IHostApplicationLifetime applicationLifetime)
        {
            var provider = app.ApplicationServices;
            var container = provider.GetAutofacRoot();
            _bootstrap.CreateContainer(container);
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseRouting();
            app.UseEndpoints(routeBuilder =>
            {
                //普通路由
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                ////区域路由
                //routeBuilder.MapAreaControllerRoute("areaRoute", "", "{area:exists}/{controller}/{action=Index}/{id?}");
                //健康检查
                routeBuilder.MapHealthChecks("/healthz", new HealthCheckOptions());
                //刷新配置
                routeBuilder.MapGet("/config/reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
                ConfigRoute(routeBuilder);
            });
            provider.GetService<MicroServiceRegister>().Regist();
            UseServices(provider);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _bootstrap.Dispose();
                provider.GetService<MicroServiceRegister>().Deregist();
            });
        }
    }
}
