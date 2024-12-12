using System;
using COLID.AppDataService.Repositories.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Repositories
{
    public static class AppDataModules
    {
        /// <summary>
        ///     This will configure the MySQL database connection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> object for registration.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> object for registration.</param>
        public static IServiceCollection AddMySqlDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO ck: handle "Database not found exception?"
            //Console.WriteLine("- Database: MySql (remote)");
            var connectionString = BuildConnectionString(configuration);
            services.AddDbContext<AppDataContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
                    {
                        mysqlOptions.CommandTimeout(5);
                        mysqlOptions.EnableRetryOnFailure(3);
                        mysqlOptions.EnableStringComparisonTranslations();
                    });
            }); // Default is scoped, which can cause errors with threading if async calls were not awaited !

            return services;
        }

        /// <summary>
        ///     This will configure the SQLite database connection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> object for registration.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> object for registration.</param>
        public static IServiceCollection AddSQLiteDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            //Console.WriteLine("- Database: SQLite (local)");
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            connection.EnableExtensions(true);
            services.AddDbContext<AppDataContext>(options => options.UseSqlite(connection));
            return services;
        }

        /// <summary>
        /// This will migrate the SQL database to the latest version.
        /// Therefore a migration must be created manually by running
        /// - initially: Add-Migration InitialCreate
        /// - after every change: Add-Migration &lt;yyyy-mm-dd_migration-name&gt;
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder" /> object for registration.</param>
        public static void UseSqlDatabaseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDataContext>();
                context.Database.Migrate();
            }
            catch (System.Exception ex)
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<DbContext>>();
                logger.LogError(ex, "An error occured and the DB migration failed");
                throw;
            }
        }

        /// <summary>
        /// This will delete the SQL database and create a new one.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder" /> object for registration.</param>
        public static void UseSqlDatabaseRecreation(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDataContext>();
                if (!context.Database.EnsureCreated())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            }
            catch (System.Exception ex)
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<DbContext>>();
                logger.LogError(ex, "An error occured and the DB migration failed");
                throw;
            }
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySQLConnection");
            Console.WriteLine("- MySQL Connection string: " + connectionString);
            var dbUser = configuration.GetValue<string>("Database:User");
            var dbPassword = configuration.GetValue<string>("Database:Password");

            return connectionString
                .Replace("{DB_USER}", dbUser, StringComparison.Ordinal)
                .Replace("{DB_PASSWORD}", dbPassword, StringComparison.Ordinal);
        }
    }
}
