using Acb.Payment;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Acb.WebApi.Test
{
    public class Startup : DStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddPayment();
            //services.AddTransient(sp => ProxyService.Proxy<IDemoService>());
            return base.ConfigureServices(services);
        }

        protected override IDictionary<string, string> DocGroups()
        {
            return new Dictionary<string, string>
            {
                {"help", "基础网关接口"},
                {"admin", "后台管理接口文档"},
                {"vehicle", "车辆管理接口文档"}
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc(routes => routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}"));
            Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
            base.Configure(app, env);
        }
    }
}
