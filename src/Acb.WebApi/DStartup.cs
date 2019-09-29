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
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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
                foreach (var (key, value) in DocGroups())
                {
                    options.SwaggerDoc(key, new OpenApiInfo
                    {
                        Title = value,
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
                options.AddSecurityDefinition("acb", new OpenApiSecurityScheme
                {
                    Description = "OAuth2授权(数据将在请求头中进行传输) 参数结构: \"Authorization: acb {token}\"",
                    Name = "Authorization", //OAuth2默认的参数名称
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "acb"
                });
                //options.OperationFilter<SwaggerFileUploadFilter>();
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
                foreach (var (key, value) in DocGroups())
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{key}/swagger.json", value);
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

        public virtual void ConfigureServices(IServiceCollection services)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            AddSwagger(services);

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services
                .AddControllers(options =>
                {
                    if (Consts.Mode != ProductMode.Dev)
                    {
                        options.Filters.Add<ActionTimingFilter>();
                    }

                    options.Filters.Add<DExceptionFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new DateTimeConverter());
                })
                .AddControllersAsServices();
            //services.AddCors();
            services.AddHealthChecks();
            //services.AddHttpContextAccessor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            MapServices(services);
        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            Bootstrap.Builder = builder;
            Bootstrap.BuilderHandler += MapServices;
            Bootstrap.Initialize();
        }

        protected virtual void ConfigRoute(IEndpointRouteBuilder builder)
        {

        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var provider = app.ApplicationServices;
            var container = provider.GetAutofacRoot();
            Bootstrap.CreateContainer(container);
            UseSwagger(app);
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

            AcbHttpContext.Configure(httpContextAccessor);

            app.UseRouting();
            app.UseCors("default");

            app.UseEndpoints(routeBuilder =>
            {
                //普通路由
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                ////区域路由
                //routeBuilder.MapAreaControllerRoute("areaRoute", "", "{area:exists}/{controller}/{action=Index}/{id?}");
                //健康检查
                routeBuilder.MapHealthChecks("/healthz", new HealthCheckOptions());
                //刷新配置
                routeBuilder.MapGet("/config/reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
                ConfigRoute(routeBuilder);
            });
            UseServices(provider);
            var liftscope = provider.GetService<IHostApplicationLifetime>();
            liftscope.ApplicationStopping.Register(Bootstrap.Dispose);
        }
    }
}
