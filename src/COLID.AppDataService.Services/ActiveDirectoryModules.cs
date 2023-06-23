using System;
using System.Collections.Generic;
using System.Text;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Services.Graph.Implementation;
using COLID.AppDataService.Services.Graph.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace COLID.AppDataService.Services
{
    public static class ActiveDirectoryModules
    {
        /// <summary>
        ///     This will configure the active directory connection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> object for registration.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> object for registration.</param>
        public static IServiceCollection AddMicrosoftActiveDirectory(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO ck: handle "Database not found exception?"
            services.AddTransient<IAuthenticationProvider, GraphAuthProvider>();
            services.AddTransient<IGraphServiceClient, GraphServiceClient>();
            services.AddTransient<IRemoteGraphService, RemoteMicrosoftGraphService>();

            return services;
        }

        /// <summary>
        ///     This will configure the in memmory active directory.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> object for registration.</param>
        /// <param name="configuration">The <see cref="IConfiguration" /> object for registration.</param>
        public static IServiceCollection AddInMemoryActiveDirectory(this IServiceCollection services)
        {
            services.AddTransient<IRemoteGraphService, InMemoryGraphService>();

            return services;
        }
    }
}
