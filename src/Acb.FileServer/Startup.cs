using Acb.FileServer.Helper;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Acb.FileServer
{
    public class Startup : DStartup
    {
        protected override void MapServices(IServiceCollection services)
        {
            services.AddSingleton<DirectoryHelper>();
            base.MapServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });
            //app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            base.Configure(app, env);
        }
    }
}
