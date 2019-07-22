using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.WebApi.Filters;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ProductMode = Acb.Core.Domain.ProductMode;

namespace Acb.WebApi
{
    public class DStartup
    {
        protected readonly DBootstrap Bootstrap;
        private string _appName;

        protected DStartup(string name = null)
        {
            Bootstrap = new DBootstrap();
            _appName = name;
        }

        #region Swagger
        /// <summary> 接口分组 </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, string> DocGroups()
        {
            return new Dictionary<string, string> { { "help", _appName } };
        }

        protected virtual void SwaggerOption(SwaggerOptions options)
        {
        }
        protected virtual void SwaggerGenOption(SwaggerGenOptions options)
        {
        }
        protected virtual void SwaggerUiOption(SwaggerUIOptions options)
        {
        }

        private void AddSwagger(IServiceCollection services)
        {
            if (Consts.Mode == ProductMode.Prod)
                return;
            var assName = Assembly.GetExecutingAssembly().GetName();
            _appName = string.IsNullOrWhiteSpace(_appName) ? assName.Name : _appName;
            services.AddSwaggerGen(options =>
            {
                foreach (var docGroup in DocGroups())
                {
                    options.SwaggerDoc(docGroup.Key, new Info
                    {
                        Title = docGroup.Value,
                        Version = $"v{assName.Version}"
                    });
                }
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    options.IncludeXmlComments(file);
                }

                options.CustomSchemaIds(t => t.FullName);
                //添加Header验证
                options.AddSecurityDefinition("acb", new ApiKeyScheme
                {
                    Description = "OAuth2授权(数据将在请求头中进行传输) 参数结构: \"Authorization: acb {token}\"",
                    Name = "Authorization",//OAuth2默认的参数名称
                    In = "header",
                    Type = "apiKey"
                });
                options.OperationFilter<SwaggerFileUploadFilter>();
                SwaggerGenOption(options);
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            if (Consts.Mode == ProductMode.Prod)
                return;
            app.UseSwagger(SwaggerOption);
            app.UseSwaggerUI(options =>
            {
                foreach (var docGroup in DocGroups())
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{docGroup.Key}/swagger.json", docGroup.Value);
                }
                SwaggerUiOption(options);
            });
        }
        #endregion

        protected virtual void MapServices(ContainerBuilder builder)
        {

        }

        protected virtual void MapServices(IServiceCollection services)
        {

        }

        protected virtual void UseServices(IServiceProvider provider)
        {

        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            LogManager.AddAdapter(new ConsoleAdapter());

            AddSwagger(services);

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
                })
                .AddControllersAsServices();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            MapServices(services);

            Bootstrap.BuilderHandler += builder =>
            {
                builder.Populate(services);
                MapServices(builder);
            };
            Bootstrap.Initialize();

            return new AutofacServiceProvider(Bootstrap.Container);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            UseSwagger(app);
            var provider = app.ApplicationServices;
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {
                //普通路由
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //区域路由
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");

                //刷新配置
                routes.MapGet("config/reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
            });
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});
            UseServices(provider);
            var liftscope = provider.GetService<IApplicationLifetime>();
            liftscope.ApplicationStopping.Register(Bootstrap.Dispose);
        }
    }
}
