using Acb.Core;
using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.MicroService
{
    public class MicroServiceStartup
    {
        private readonly DBootstrap _bootstrap;

        public MicroServiceStartup()
        {
            _bootstrap = DBootstrap.Instance;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _bootstrap.BuilderHandler += builder => { builder.Populate(services); };
            _bootstrap.Initialize();

            MicroServiceRegister.Regist();
            LogManager.AddAdapter(new ConsoleAdapter());
            return new AutofacServiceProvider(_bootstrap.Container);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes => { routes.Routes.Add(new MicroServiceRouter()); });
        }
    }
}
