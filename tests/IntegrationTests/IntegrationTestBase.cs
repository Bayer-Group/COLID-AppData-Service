using System;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Repositories.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly SqliteConnection _connection;
        protected readonly TestDataContextSeeder _seeder;
        protected readonly IMapper _mapper;
        protected readonly ITestOutputHelper _output;

        protected IntegrationTestBase(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new MappingProfiles());
            });

            _mapper = config.CreateMapper();
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _seeder = new TestDataContextSeeder(GetDbContextOptions());
        }

        protected DbContextOptions<AppDataContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<AppDataContext>()
                .UseSqlite(_connection)
                .Options;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
