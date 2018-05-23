using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.WebApi.Filters;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.WebApi
{
    public class DStartup
    {
        private readonly DBootstrap _bootstrap;

        protected DStartup()
        {
            _bootstrap = DBootstrap.Instance;
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    if (Consts.Mode != ProductMode.Dev)
                    {
                        options.Filters.Add<ActionTimingFilter>();
                    }

                    //自定义异常捕获
                    options.Filters.Add<DExceptionFilter>();
                })
                .AddJsonOptions(opts =>
                {
                    //json序列化处理
                    opts.SerializerSettings.Converters.Add(new DateTimeConverter());
                });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _bootstrap.BuilderHandler += builder => { builder.Populate(services); };
            _bootstrap.Initialize();
            LogManager.AddAdapter(new ConsoleAdapter());
            return new AutofacServiceProvider(_bootstrap.Container);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc();
            var liftscope = app.ApplicationServices.GetService<IApplicationLifetime>();
            liftscope.ApplicationStopping.Register(DBootstrap.Instance.Dispose);
        }
    }
}
