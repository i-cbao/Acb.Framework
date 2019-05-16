using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Middleware.DatabaseManager
{
    public class Startup : DStartup
    {
        public Startup()
        {
        }

        protected override void MapServices(IServiceCollection services)
        {
            //services.AddDatabase("default", "public");
            base.MapServices(services);
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            base.Configure(app, env);
        }
    }
}
