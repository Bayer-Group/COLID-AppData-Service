using Azure.Identity;
using COLID.AppDataService.Services.Graph.Implementation;
using COLID.AppDataService.Services.Graph.Interfaces;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Client;

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
            var graphConfig = configuration.GetSection("AzureAd");
            var clientSecretCredential = new ClientSecretCredential(graphConfig["TenantId"], graphConfig["ClientId"], graphConfig["ClientSecret"]);
            services.AddSingleton(sp => new GraphServiceClient(clientSecretCredential));
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
