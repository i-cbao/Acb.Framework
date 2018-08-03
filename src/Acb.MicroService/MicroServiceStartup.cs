using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.MicroService.Filters;
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

namespace Acb.MicroService
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
            services.AddMvc(options =>
            {
                //自定义异常捕获
                options.Filters.Add<DExceptionFilter>();
            });

            //services.TryAddSingleton<IRegister, ConsulRegister>();
            MapServices(services);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _bootstrap.BuilderHandler += builder =>
            {
                builder.Populate(services);
                MapServices(builder);
            };
            _bootstrap.Initialize();

            MicroServiceRegister.Regist();
            LogManager.AddAdapter(new ConsoleAdapter());
            return new AutofacServiceProvider(_bootstrap.Container);
        }


        /// <summary> 配置应用 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="applicationLifetime"></param>
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            var provider = app.ApplicationServices;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {
                routes.MapGet("healthy", async ctx => await ctx.Response.WriteAsync("ok"));
                routes.MapGet("reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
                routes.MapGet("micro", async ctx => await MicroServiceRunner.Methods(ctx));
                routes.MapPost("micro/{contract}/{method}", (request, response, route) =>
                {
                    route.Values.TryGetValue("contract", out var contract);
                    route.Values.TryGetValue("method", out var method);
                    return MicroServiceRunner.MicroTask(request, response, $"{contract}/{method}");
                });
                //普通路由
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //区域路由
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
            });
            UseServices(provider);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _bootstrap.Dispose();
                MicroServiceRegister.Deregist();
            });
        }
    }
}
