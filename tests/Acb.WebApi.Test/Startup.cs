using Acb.Payment;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Acb.WebApi.Test
{
    public class Startup :
        DStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddPayment();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("help", new Info
                {
                    Title = "Api Help Page",
                    Version = "v1.0"
                });
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    option.IncludeXmlComments(file);
                }
            });
            return base.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/help/swagger.json", "Fiver.Api Help Endpoint");
            });
            Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
            base.Configure(app, env);
        }
    }
}
