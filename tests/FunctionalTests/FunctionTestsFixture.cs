using System;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Services;
using COLID.AppDataService.Tests.Integration;
using COLID.Cache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Tests.Functional
{
    public class FunctionTestsFixture : WebApplicationFactory<Startup>
    {
        public DbContextOptions<AppDataContext> DbContextOptions { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "appsettings.Testing.json");
                conf.AddUserSecrets<Startup>();
            });

            builder.ConfigureServices(services =>
            {
                services.AddAutoMapper(typeof(MappingProfiles));

                // Create a new service provider.
                var provider = services
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory
                // database for testing.
                services.AddDbContext<AppDataContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(provider);
                    DbContextOptions = (DbContextOptions<AppDataContext>)options.Options;
                });

                services.AddInMemoryActiveDirectory();
                services.AddNoCacheModule();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDataContext>();
                var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();
                var logger = scopedServices.GetRequiredService<ILogger<FunctionTestsFixture>>();

                try
                {
                    db.Database.EnsureCreated();
                    new TestDataContextSeeder(DbContextOptions).SeedAll();
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                    throw;
                }
            });
        }
    }
}
