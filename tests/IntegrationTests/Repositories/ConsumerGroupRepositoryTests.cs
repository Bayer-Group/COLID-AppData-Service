using System;
using System.Linq;
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
    public class ConsumerGroupRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IConsumerGroupRepository _repo;

        public ConsumerGroupRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new ConsumerGroupRepository(_context);

            _context.Database.EnsureCreated();

            _seeder.SeedConsumerGroups();
        }

        [Fact]
        public void GetOneByUri_Should_ReturnConsumerGroup()
        {
            var expected = TestData.GetPreconfiguredConsumerGroups().First();
            var actual = _repo.GetOne(expected.Uri);
            Assert.NotNull(actual);
            Assert.NotNull(actual.CreatedAt);
            Assert.NotNull(actual.ModifiedAt);
            Assert.Equal(expected.Uri.AbsoluteUri, actual.Uri.AbsoluteUri);
        }

        [Fact]
        public void GetOneByUri_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repo.GetOne((Uri)null));
        }

        [Fact]
        public void GetOneByUri_Should_ThrowException_IfEntityDoesNotExist()
        {
            Assert.Throws<EntityNotFoundException>(() => _repo.GetOne(new Uri("http://non.existing.consumer.group")));
        }

        [Fact]
        public void GetOneByUri_Should_ThrowException_IfUriFormatIsInvalid()
        {
            Assert.Throws<UriFormatException>(() => _repo.GetOne(new Uri("meeeeh")));
        }

        [Fact]
        public void TryGetOneByUri_Should_ReturnFalse_IfEntityDoesNotExist()
        {
            var result = _repo.TryGetOne(new Uri("http://non.exiting.uri"), out var entity);
            Assert.False(result);
            Assert.Null(entity);
        }

        [Fact]
        public void TryGetOneByUri_Should_ReturnTrue_IfEntityExists()
        {
            var expected = TestData.GetPreconfiguredConsumerGroups().First();
            var result = _repo.TryGetOne(expected.Uri, out var entity);
            Assert.NotNull(result);
            Assert.True(result);

            Assert.NotNull(entity.CreatedAt);
            Assert.NotNull(entity.ModifiedAt);
            Assert.Equal(expected.Uri.AbsoluteUri, entity.Uri.AbsoluteUri);
        }

        [Fact]
        public void TryGetOneByUri_Should_ThrowException_IfUriFormatIsInvalid()
        {
            Assert.Throws<UriFormatException>(() => _repo.TryGetOne(new Uri("meeeeh"), out var _));
        }
    }
}
