using System;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Tests.Unit;
using COLID.Exception.Models.Business;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class ColidEntrySubscriptionRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IColidEntrySubscriptionRepository _repo;

        public ColidEntrySubscriptionRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new ColidEntrySubscriptionRepository(_context);

            _context.Database.EnsureCreated();

            _seeder.SeedColidEntrySubscriptions();
        }

        [Fact]
        public void GetOneByUserIdAndUri_Should_ReturnColidEntrySubscription()
        {
            var user = TestData.GenerateRandomUser(); // will be created with add of subscription next line
            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            _seeder.Add(subscription);

            var actual = _repo.GetOne(user.Id, subscription.ColidPidUri);
            Assert.NotNull(actual);
            Assert.NotNull(actual.CreatedAt);
            Assert.NotNull(actual.ModifiedAt);
            Assert.Equal(subscription.ColidPidUri.AbsoluteUri, actual.ColidPidUri.AbsoluteUri);
        }

        [Fact]
        public void GetOneByUserIdAndUri_Should_ThrowException_IfUriIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repo.GetOne(Guid.NewGuid(), null));
        }

        [Fact]
        public void GetOneByUserIdAndUri_Should_ThrowException_IfEntityDoesNotExist()
        {
            Assert.Throws<EntityNotFoundException>(() => _repo.GetOne(Guid.NewGuid(), new Uri("http://non.existing.subscription")));
        }

        [Fact]
        public void GetOneByUserIdAndUri_Should_ThrowException_IfUriFormatIsInvalid()
        {
            Assert.Throws<UriFormatException>(() => _repo.GetOne(Guid.NewGuid(), new Uri("meeeeh")));
        }

        [Fact]
        public void TryGetAllUsers_Should_ReturnFalse_IfEntityDoesNotExist()
        {
            var result = _repo.TryGetAllUsers(new Uri("http://non.exiting.uri"), out var entity);
            Assert.False(result);
            Assert.Empty(entity);
        }

        [Fact]
        public void TryGetAllUsers_Should_ReturnTrue_IfEntityExists()
        {
            var user = TestData.GenerateRandomUser(); // will be created with add of subscription next line
            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            subscription = _seeder.Add(subscription);

            var result = _repo.TryGetAllUsers(subscription.ColidPidUri, out var entity);
            Assert.NotNull(result);
            Assert.True(result);

            Assert.NotNull(entity);
            Assert.Equal(entity.First().Id, subscription.User.Id);
        }

        [Fact]
        public void Delete_Should_CallDeleteRange_IfEntitiesFound()
        {
            var user = TestData.GenerateRandomUser(); // will be created with add of subscription next line
            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            _seeder.Add(subscription);

            var subscriptionToDelete = new ColidEntrySubscriptionDto() {ColidPidUri = subscription.ColidPidUri};
            _repo.Delete(subscriptionToDelete);

            var foundSubscription = _seeder.GetAll<ColidEntrySubscription>()
                .FirstOrDefault(x => x.ColidPidUri.AbsoluteUri == subscription.ColidPidUri.AbsoluteUri);
            Assert.Null(foundSubscription);
        }

        [Fact]
        public void Delete_Should_DoNothing_IfNoEntitesFound()
        {
            var subscriptionToDelete = new ColidEntrySubscriptionDto() { ColidPidUri = new Uri("http://www.iliketo.meh") };
            _repo.Delete(subscriptionToDelete);
        }

        [Fact]
        public void Delete_Should_ThrowException_IfArgumentIsNull()
        {
            ColidEntrySubscriptionDto nullDto = null;
            Assert.Throws<ArgumentNullException>(() => _repo.Delete(nullDto));
        }

    }
}
