using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Framework;
using Acb.Framework.Logging;
using Acb.WebApi.Filters;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace Acb.WebApi
{
    public class DStartup
    {
        private readonly DBootstrap _bootstrap;
        private string _appName;

        protected DStartup(string name = null)
        {
            _bootstrap = new DBootstrap();
            _appName = name;
        }

        private void AddSwagger(IServiceCollection services)
        {
            if (Consts.Mode == ProductMode.Prod)
                return;
            var assName = Assembly.GetExecutingAssembly().GetName();
            _appName = string.IsNullOrWhiteSpace(_appName) ? assName.Name : _appName;
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("help", new Info
                {
                    Title = _appName,
                    Version = $"v{assName.Version}"
                });
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    option.IncludeXmlComments(file);
                }

                option.CustomSchemaIds(t => t.FullName);
                option.AddSecurityDefinition("acb", new ApiKeyScheme
                {
                    Description = "OAuth2授权(数据将在请求头中进行传输) 参数结构: \"Authorization: acb {token}\"",
                    Name = "Authorization",//OAuth2默认的参数名称
                    In = "header",
                    Type = "apiKey"
                });
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            if (Consts.Mode == ProductMode.Prod)
                return;
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/help/swagger.json", _appName);
            });
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
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
                });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _bootstrap.BuilderHandler += builder => { builder.Populate(services); };
            _bootstrap.Initialize();
            LogManager.AddAdapter(new ConsoleAdapter());
            return new AutofacServiceProvider(_bootstrap.Container);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            UseSwagger(app);
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            AcbHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {
                routes.MapGet("reload", async ctx =>
                {
                    ConfigHelper.Instance.Reload();
                    await ctx.Response.WriteAsync("ok");
                });
            });
            var liftscope = app.ApplicationServices.GetService<IApplicationLifetime>();
            liftscope.ApplicationStopping.Register(_bootstrap.Dispose);
        }
    }
}
