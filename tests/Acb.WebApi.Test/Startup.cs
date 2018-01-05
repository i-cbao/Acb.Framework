using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Acb.WebApi.Test
{
    public class Startup : DStartup
    {
        public Startup(IConfiguration configuration)
        : base(Assembly.GetExecutingAssembly())
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return base.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            base.Configure(app, env);
        }
    }
}
