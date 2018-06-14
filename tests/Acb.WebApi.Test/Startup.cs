using Acb.Demo.Contracts;
using Acb.MicroService.Client;
using Acb.Payment;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.WebApi.Test
{
    public class Startup : DStartup
    {
        public Startup() : base("基础网关接口") { }
        // This method gets called by the runtime. Use this method to add services to the container.
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddPayment();
            services.AddTransient(sp => ProxyService.Proxy<IDemoService>());
            return base.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
            base.Configure(app, env);
        }
    }
}
