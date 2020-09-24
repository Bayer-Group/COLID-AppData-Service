using System;
using System.Linq;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class MessageTemplateRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IMessageTemplateRepository _repo;

        public MessageTemplateRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new MessageTemplateRepository(_context);

            _context.Database.EnsureCreated();

            _seeder.SeedMessageTemplates();
        }

        [Fact]
        public void GetOneByType_Should_ReturnMessageTemplate()
        {
            var expected = TestData.GetPreconfiguredMessageTemplates().Where(t => t.Type.Equals(MessageType.ColidEntrySubscriptionUpdate)).FirstOrDefault();
            var actual = _repo.GetOne(MessageType.ColidEntrySubscriptionUpdate);

            Assert.NotNull(actual);
            Assert.NotNull(actual.Id);
            Assert.NotNull(expected);

            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.Body, actual.Body);
            Assert.Equal(expected.Subject, actual.Subject);
        }

        [Fact]
        public void TryGetOneByType_Should_ReturnTrue_IfEntityExists()
        {
            var existing = TestData.GetPreconfiguredMessageTemplates().First();
            var result = _repo.TryGetOne(existing.Type, out var entity);
            Assert.NotNull(result);
            Assert.True(result);

            Assert.NotNull(entity.Id);
            Assert.NotNull(entity.Type);
            Assert.NotNull(entity.Subject);
            Assert.NotNull(entity.Body);
        }

        [Fact]
        public void TryGetOneByType_Should_ReturnFalse_IfEntityDoesNotExist()
        {
            _seeder.ClearMessageTemplates();
            var result = _repo.TryGetOne(MessageType.StoredQueryResult, out var entity);
            Assert.False(result);
            Assert.Null(entity);
            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void TryGetOneByDto_Should_ReturnTrue_IfEntityExists()
        {
            _seeder.ResetMessageTemplates();
            var existing = TestData.GetPreconfiguredMessageTemplates().Where(t => t.Type.Equals(MessageType.ColidEntrySubscriptionUpdate)).FirstOrDefault();
            var dto = new MessageTemplateBuilder().WithType(existing.Type).WithSubject(existing.Subject).WithBody(existing.Body).BuildDto();

            var result = _repo.TryGetOne(dto, out var entity);
            Assert.NotNull(result);
            Assert.True(result);

            Assert.NotNull(entity.Id);
        }

        [Fact]
        public void TryGetOneByDto_Should_ReturnFalse_IfEntityDoesNotExist()
        {
            _seeder.ClearMessageTemplates();
            var dto = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("bliblablubb123123").WithBody("some nonsense here").BuildDto();
            var result = _repo.TryGetOne(dto, out var entity);
            Assert.False(result);
            Assert.Null(entity);
            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void TryGetOneByDto_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repo.TryGetOne(null, out var entity));
        }
    }
}
