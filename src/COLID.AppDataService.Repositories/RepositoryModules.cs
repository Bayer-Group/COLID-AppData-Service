﻿using System;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
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

            services.AddTransient<IColidEntrySubscriptionRepository, ColidEntrySubscriptionRepository>();
            services.AddTransient<IConsumerGroupRepository, ConsumerGroupRepository>();
            services.AddTransient(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IWelcomeMessageRepository, WelcomeMessageRepository>();

            return services;
        }
    }
}
