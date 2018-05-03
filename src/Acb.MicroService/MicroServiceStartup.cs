using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.MicroService.Filters;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Acb.MicroService
{
    public class MicroServiceStartup
    {
        private readonly DBootstrap _bootstrap;

        public MicroServiceStartup()
        {
            _bootstrap = DBootstrap.Instance;
        }

        /// <summary> 配置服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                //自定义异常捕获
                options.Filters.Add<DExceptionFilter>();
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _bootstrap.BuilderHandler += builder => { builder.Populate(services); };
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddConsole((p, l) => l >= LogLevel.Warning);
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {
                routes.MapGet("healthy", async ctx => await ctx.Response.WriteAsync("ok"));
                routes.MapGet("micro", async ctx => await MicroServiceRunner.Methods(ctx));
                routes.MapPost("micro/{contract}/{method}", (request, response, route) =>
                {
                    route.Values.TryGetValue("contract", out var contract);
                    route.Values.TryGetValue("method", out var method);
                    return MicroServiceRunner.MicroTask(request, response, $"{contract}/{method}");
                });
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                DBootstrap.Instance.Dispose();
                if (Consts.Mode == ProductMode.Prod)
                    MicroServiceRegister.Deregist();
            });
        }
    }
}
