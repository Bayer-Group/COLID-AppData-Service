﻿using System;
using System.Collections.Generic;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Repositories;
using COLID.AppDataService.Services;
using COLID.Exception;
using COLID.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using COLID.Swagger;
using COLID.Cache.Configuration;

namespace COLID.AppDataService
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            Configuration = configBuilder.Build();
            Environment = env;

            Console.WriteLine("- DocumentationUrl = " + Configuration.GetValue<string>("ColidSwaggerOptions:DocumentationUrl"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddHealthChecks();
            services.AddControllers(o =>
                    o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter()))
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddColidSwaggerGeneration(Configuration);
            services.AddServiceModules(Configuration);
            services.AddRepositoryModules(Configuration);
            services.AddAuthorizationModule(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(
                options => options.SetIsOriginAllowed(x => _ = true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseExceptionMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            app.UseColidSwaggerUI(Configuration);
        }

        private static CachingJsonSerializerSettings GetSerializerSettings()
        {
            var serializerSettings = new CachingJsonSerializerSettings
            {
                Converters = new List<JsonConverter>(),
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            return serializerSettings;
        }
    }
}
