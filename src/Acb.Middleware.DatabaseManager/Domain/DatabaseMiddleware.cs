using System.Data.SQLite;
using Acb.Core.Domain;
using Acb.Dapper;
using Acb.Middleware.DatabaseManager.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Acb.Middleware.DatabaseManager.Domain
{
    public static class DatabaseMiddleware
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string configName = null, string dbSchema = null)
        {
            services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(configName));
            services.AddScoped<IDatabaseService>(provider =>
            {
                var unitOfWork = provider.GetService<IUnitOfWork>();
                switch (unitOfWork.Connection)
                {
                    case NpgsqlConnection _:
                        return new PostgreSqlService(unitOfWork, dbSchema);
                    case MySqlConnection _:
                        return new MySqlService(unitOfWork);
                    case SQLiteConnection _:
                        return new SqliteService(unitOfWork);
                }

                return new MsSqlService(unitOfWork);
            });
            return services;
        }

        public static IDatabaseService GetService(this string name, string dbSchema = null)
        {
            var unitOfWork = new UnitOfWork(name);
            switch (unitOfWork.Connection)
            {
                case NpgsqlConnection _:
                    return new PostgreSqlService(unitOfWork, dbSchema);
                case MySqlConnection _:
                    return new MySqlService(unitOfWork);
                case SQLiteConnection _:
                    return new SqliteService(unitOfWork);
            }

            return new MsSqlService(unitOfWork);
        }
    }
}
