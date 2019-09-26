using Acb.Core.Extensions;
using Acb.Core.Monitor;
using Acb.MicroService.Client;
using Acb.RabbitMq;
using Acb.WebApi.Test.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Acb.WebApi.Test
{
    public class Startup : DStartup
    {
        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        protected override void MapServices(IServiceCollection services)
        {
            services.AddDbContext<DbContext>(options => { });

            services.AddRabbitMqEventBus("spartner");
            services.AddRabbitMqEventBus();

            //services.AddCors(opts =>
            //    opts.AddPolicy("mhubs", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            services.AddMonitor(typeof(LoggerMonitor));
            services.AddSignalR();
            //services.AddSingleton(provider =>
            //{
            //    var finder = provider.GetService<ITypeFinder>();
            //    return ProxyService.Proxy<IDemoService>();
            //});
            //services.AddMicroRouter();
            services.AddMicroClient();
            //IdentityServer4
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(OAuthData.GetIdentityResourceResources())
            //    .AddInMemoryApiResources(OAuthData.GetApiResources())
            //    .AddInMemoryClients(OAuthData.GetClients())
            //    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            //    .AddProfileService<ProfileService>();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            //provider.SubscribeAt();
            base.UseServices(provider);
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

        /// <summary> 路由信息 </summary>
        /// <param name="builder"></param>
        protected override void ConfigRoute(IEndpointRouteBuilder builder)
        {
            //区域路由
            builder.MapAreaControllerRoute("adminRoute", "admin", "admin/{controller}/{action=Index}/{id?}");
            builder.MapAreaControllerRoute("vehicleRoute", "vehicle", "vehicle/{controller}/{action=Index}/{id?}");
            builder.MapHub<MessageHub>("/mhub");
            base.ConfigRoute(builder);
        }

        /// <summary> 配置应用 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseStaticFiles();
            //app.UseCors("mhubs");
            //app.UseWebSockets();

            //app.UseIdentityServer();
            base.Configure(app, env);
        }
    }
}
