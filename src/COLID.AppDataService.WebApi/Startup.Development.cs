using COLID.AppDataService.Repositories;
using COLID.AppDataService.Services;
using COLID.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace COLID.AppDataService
{
    public partial class Startup
    {
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services);

            services.AddMySqlDatabaseContext(Configuration);
            services.AddMicrosoftActiveDirectory(Configuration);
            services.AddDistributedCacheModule(Configuration, GetSerializerSettings());
        }

        public void ConfigureDevelopment(IApplicationBuilder app)
        {
            app.UseSqlDatabaseMigration();
            Configure(app);
        }
    }
}
