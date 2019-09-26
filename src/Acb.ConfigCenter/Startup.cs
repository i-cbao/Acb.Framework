using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace Acb.ConfigCenter
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVVV";
                o.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddSwaggerGen(s =>
            {
                //填充UI内容
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    s.SwaggerDoc(description.GroupName,
                        new Info()
                        {
                            Title = $"体检微服务接口 v{description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = "微服务框架-切换版本请点右上角版本切换"
                        }
                    );
                }

                //生成API XML文档
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var xmlPath = Path.Combine(basePath, typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml");
                s.IncludeXmlComments(xmlPath);
            });
            services.AddMvc();
            services.AddConfigManager();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = new[] { "index.html" } }
            });
            //app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
                //区域路由
                //builder.MapAreaControllerRoute("areaRoute", string.Empty, pattern: "{area:exists}/{controller}/{action=Index}/{id?}");
                //普通路由
                builder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
