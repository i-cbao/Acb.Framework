using Acb.MicroService.Client;
using Acb.Payment;
using Acb.RabbitMq;
using Acb.WebApi.Test.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Acb.WebApi.Test.OAuth;
using Microsoft.EntityFrameworkCore;

namespace Acb.WebApi.Test
{
    public class GatewayStartup : DStartup
    {
        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        protected override void MapServices(IServiceCollection services)
        {
            services.AddDbContext<DbContext>(options => { });
            services.AddRabbitMqEventBus();
            services.AddPayment();

            services.AddCors(opts =>
                opts.AddPolicy("mhubs", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            services.AddSignalR();
            //services.AddSingleton(provider =>
            //{
            //    var finder = provider.GetService<ITypeFinder>();
            //    return ProxyService.Proxy<IDemoService>();
            //});
            services.AddMicroClient();
            //IdentityServer4
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(OAuthData.GetIdentityResourceResources())
                .AddInMemoryApiResources(OAuthData.GetApiResources())
                .AddInMemoryClients(OAuthData.GetClients())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddProfileService<ProfileService>();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            //var ioc = (IocManager)provider.GetService<IIocManager>();
            //ioc.MapService(b =>
            //{
            //    var serviceType = typeof(IMicroService);
            //    var types = provider.GetService<ITypeFinder>()
            //        .Find(t => serviceType.IsAssignableFrom(t) && t != serviceType);
            //    foreach (var type in types)
            //    {
            //        b.RegisterInstance(ProxyService.Proxy(type)).AsImplementedInterfaces().SingleInstance();
            //    }
            //});
            //provider.SubscriptAt();
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
            app.UseIdentityServer();
            base.Configure(app, env);
        }
    }
}
