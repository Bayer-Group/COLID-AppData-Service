using COLID.AppDataService.Repositories;
using COLID.AppDataService.Services;
using COLID.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace COLID.AppDataService
{
    public partial class Startup
    {
        public void ConfigureLocalServices(IServiceCollection services)
        {
            // override builder for user secrets
            var builder = new ConfigurationBuilder()
                .AddConfiguration(Configuration)
                .AddUserSecrets<Startup>();
            Configuration = builder.Build();

            ConfigureServices(services);

            var useSQLite = Configuration.GetValue<bool>("UseSQLite");
            if (useSQLite)
            {
                services.AddSQLiteDatabaseContext(Configuration);
            }
            else
            {
                services.AddMySqlDatabaseContext(Configuration);
            }

            var useInMemoryGraph = Configuration.GetValue<bool>("UseInMemoryGraph");
            if (useInMemoryGraph)
            {
                services.AddInMemoryActiveDirectory();
            }
            else
            {
                services.AddMicrosoftActiveDirectory(Configuration);
            }

            services.AddCacheModule(Configuration, GetSerializerSettings());
        }

        public void ConfigureLocal(IApplicationBuilder app)
        {
            var useSQLite = Configuration.GetValue<bool>("UseSQLite");
            if (useSQLite)
            {
                app.UseSqlDatabaseRecreation();
            }
            else
            {
                app.UseSqlDatabaseMigration();
            }

            Configure(app);
        }
    }
}
