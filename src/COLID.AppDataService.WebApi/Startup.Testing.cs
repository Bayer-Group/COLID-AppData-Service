using COLID.AppDataService.Repositories;
using COLID.AppDataService.Services;
using COLID.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace COLID.AppDataService
{
    public partial class Startup
    {
        public void ConfigureTestServices(IServiceCollection services)
        {
            ConfigureServices(services);

            services.AddInMemoryActiveDirectory();
            services.AddSQLiteDatabaseContext(Configuration);
            services.AddNoCacheModule();
        }

        public void ConfigureTest(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Configure(app, env);
        }
    }
}
