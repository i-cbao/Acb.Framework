using Acb.Core.Domain;
using Acb.Dapper;
using Acb.Middleware.DatabaseManager.Domain;
using Acb.Middleware.DatabaseManager.Domain.Services;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Npgsql;

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
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            base.Configure(app, env);
        }
    }
}
