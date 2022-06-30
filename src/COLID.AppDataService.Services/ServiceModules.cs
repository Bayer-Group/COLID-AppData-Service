using System;
using COLID.AppDataService.Services.Graph.Implementation;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Configuration;

namespace COLID.AppDataService.Services
{
    public static class ServiceModules
    {
        /// <summary>
        /// This will register all the supported functionality by Repositories module.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration</param>
        public static IServiceCollection AddServiceModules(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IColidEntrySubscriptionService, ColidEntrySubscriptionService>();
            services.AddTransient<IConsumerGroupService, ConsumerGroupService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IMessageTemplateService, MessageTemplateService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWelcomeMessageService, WelcomeMessageService>();
            services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();
            services.AddTransient<IRemoteSearchService, RemoteSearchService>();

            services.Configure<SearchServiceTokenOptions>(configuration.GetSection("SearchServiceTokenOptions"));


            return services;
        }
    }
}
