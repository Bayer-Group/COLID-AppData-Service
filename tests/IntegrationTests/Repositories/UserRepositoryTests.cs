using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
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
    public class UserRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IUserRepository _repo;
        private readonly IEnumerable<User> _userList = TestData.GetPreconfiguredUsers();
        private readonly IEnumerable<ConsumerGroup> _consumerGroupList = TestData.GetPreconfiguredConsumerGroups();
        private readonly IEnumerable<SearchFilterEditor> _searchFilterEditorList = TestData.GetPreconfiguredSearchFilterEditor();

        public UserRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new UserRepository(_context);

            _context.Database.EnsureCreated();

            // Seed TestData
            _seeder.SeedUsers();
            _seeder.SeedConsumerGroups();
            _seeder.AssignDefaultConsumerGroupToUser(_userList.First(), _consumerGroupList.First());
            _seeder.SeedSearchFiltersEditor();
            _seeder.AssignSearchFilterEditorToUser(_userList.First(), _searchFilterEditorList.First());
        }

        [Fact]
        public async Task GetDefaultConsumerGroupAsync_Should_ReturnDefaultConsumerGroup()
        {
            var user = _userList.First();
            var expected = _consumerGroupList.First();
            var actual = await _repo.GetDefaultConsumerGroupAsync(user.Id);
            Assert.NotNull(actual);
            Assert.NotNull(actual.CreatedAt);
            Assert.NotNull(actual.ModifiedAt);
            Assert.Equal(expected.Uri.AbsoluteUri, actual.Uri.AbsoluteUri);
        }

        [Fact]
        public void GetDefaultConsumerGroupAsync_Should_ThrowException_IfUserDoesNotExist()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repo.GetDefaultConsumerGroupAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetSearchFilterEditorAsync_Should_ReturnSearchFilterEditor()
        {
            var user = _userList.First();
            var expected = _searchFilterEditorList.First();
            var actual = await _repo.GetSearchFilterEditorAsync(user.Id);
            Assert.NotNull(actual);
            Assert.NotNull(actual.CreatedAt);
            Assert.NotNull(actual.ModifiedAt);
            Assert.Equal(expected.FilterJson, actual.FilterJson);
        }

        [Fact]
        public void GetSearchFilterEditorAsync_Should_ThrowException_IfUserDoesNotExist()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repo.GetSearchFilterEditorAsync(Guid.NewGuid()));
        }
    }
}
