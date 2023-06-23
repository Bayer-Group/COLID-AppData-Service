using System;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace COLID.AppDataService.Repositories
{
    public static class RepositoryModules
    {
        /// <summary>
        /// This method will register all the supported functions by Repository modules.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration</param>
        public static IServiceCollection AddRepositoryModules(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IGenericRepository, GenericRepository>();
            services.AddTransient<IApplicationVersionRepository, ApplicationVersionRepository>();

            return services;
        }
    }
}
