using System;
using System.Collections.Generic;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Tests.Unit;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class WelcomeMessagesRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IWelcomeMessageRepository _repo;
        private readonly IEnumerable<WelcomeMessage> _welcomeMessageList = TestData.GetPreconfiguredWelcomeMessages();

        public WelcomeMessagesRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new WelcomeMessageRepository(_context);

            _context.Database.EnsureCreated();

            // Seed TestData
            _seeder.SeedWelcomeMessages();
        }

        [Fact]
        public void GetOne_Should_ReturnOneWelcomeMessage()
        {
            var expected = _welcomeMessageList.First();
            var actual = _repo.GetOne(expected.Type);
            Assert.NotNull(actual);
            Assert.NotNull(actual.CreatedAt);
            Assert.NotNull(actual.ModifiedAt);
            Assert.Equal(expected.Type, actual.Type);
        }
    }
}
