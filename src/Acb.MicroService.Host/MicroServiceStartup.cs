using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.MicroService.Host.Filters;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            LogManager.AddAdapter(new ConsoleAdapter());

            services.AddMvc(options =>
            {
                //自定义异常捕获
                options.Filters.Add<DExceptionFilter>();
            }).AddControllersAsServices();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<MicroServiceRegister>();
            services.AddSingleton<MicroServiceRunner>();

            //使用Json编解码
            services.AddJsonCodec();

            //使用服务注册发现
            services.AddMicroRouter();

            MapServices(services);

            _bootstrap.BuilderHandler += builder =>
            {
                builder.Populate(services);
                MapServices(builder);
            };
            _bootstrap.Initialize();
            return new AutofacServiceProvider(_bootstrap.Container);
        }


        /// <summary> 配置应用 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="applicationLifetime"></param>
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            var provider = app.ApplicationServices;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {
                //健康检测
                routes.MapGet("healthy", async ctx => await ctx.Response.WriteAsync("ok"));
                //重新加载配置
                routes.MapGet("reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
                //普通路由
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //区域路由
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
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
