using System;
using Acb.Payment;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Acb.WebApi.Test.Hubs;

namespace Acb.WebApi.Test
{
    public class GatewayStartup : DStartup
    {
        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        protected override void MapServices(IServiceCollection services)
        {
            services.AddPayment();
            services.AddCors(opts =>
                opts.AddPolicy("mhubs", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
            services.AddSignalR();
            base.MapServices(services);
        }

        /// <summary> 接口文档 </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> DocGroups()
        {
            return new Dictionary<string, string>
            {
                {"help", "基础网关接口"},
                {"admin", "后台管理接口文档"},
                {"vehicle", "车辆管理接口文档"}
            };
        }

        /// <summary> 配置应用 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("mhubs");
            app.UseSignalR(route =>
            {
                route.MapHub<MessageHub>("/mhub");
            });
            app.UseWebSockets();
            app.UseStaticFiles();
            Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
            base.Configure(app, env);
        }
    }
}
