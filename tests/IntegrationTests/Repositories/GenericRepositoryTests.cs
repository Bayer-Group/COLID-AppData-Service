using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using COLID.Exception.Models.Business;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class GenericRepositoryTest : IntegrationTestBase
    {
        private readonly AppDataContext _context;

        ///  Tests all generic repository operations with INT as identifier type and MessageTemplate as entity type
        private readonly IGenericRepository<MessageTemplate, int> _intRepo;

        private readonly IEnumerable<MessageTemplate> _intList = TestData.GetPreconfiguredMessageTemplates();

        public GenericRepositoryTest(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _intRepo = new GenericRepository<MessageTemplate, int>(_context);
            _context.Database.EnsureCreated();

            _seeder.SeedMessageTemplates();
        }

        #region [Synchronous tests]

        [Fact]
        public void FindAll_Should_ReturnQueryableObject()
        {
            Assert.NotNull(_intRepo.FindAll());
        }

        [Fact]
        public void FindByCondition_Should_ReturnQueryableObject()
        {
            Assert.NotNull(_intRepo.FindByCondition(e => e.IsNotEmpty()));
        }

        [Fact]
        public void FindByCondition_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.FindByCondition(null));
        }

        [Fact]
        public void GetAll_Should_ReturnEntities()
        {
            int amountOfEntities = _intList.Count();
            var entitiesFromRepository = _intRepo.GetAll();

            Assert.NotNull(entitiesFromRepository);
            Assert.Equal(amountOfEntities, entitiesFromRepository.Count());
        }

        [Fact]
        public void GetAll_Should_ReturnEmptyContent_IfNoEntitesExist()
        {
            _seeder.ClearMessageTemplates();

            var entitiesFromRepository = _intRepo.GetAll();
            Assert.NotNull(entitiesFromRepository);
            Assert.Equal(0, entitiesFromRepository.Count());

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void GetOne_Should_ReturnOneEntity()
        {
            var expected = _intList.First();
            var actualEntity = _intRepo.GetOne(expected.Id);

            Assert.NotNull(actualEntity);
            Assert.Equal(expected.Type, actualEntity.Type);
            Assert.Equal(expected.Subject, actualEntity.Subject);
            Assert.Equal(expected.Body, actualEntity.Body);
        }

        [Fact]
        public void GetOne_Should_ThrowException_IfEntityDoesNotExist()
        {
            Assert.Throws<EntityNotFoundException>(() => _intRepo.GetOne(999));
        }

        [Fact]
        public void TryGetOne_Should_ReturnFalse_IfEntityDoesNotExist()
        {
            // also checks null of out param
            var result = _intRepo.TryGetOne(999, out var entity);
            Assert.False(result);
            Assert.Null(entity);
        }

        [Fact]
        public void TryGetOne_Should_ReturnTrue_IfEntityExists()
        {
            // also check entity of out param
            var expected = _intList.First();
            var result = _intRepo.TryGetOne(expected.Id, out var entity);
            Assert.NotNull(result);
            Assert.True(result);

            Assert.NotNull(entity.CreatedAt);
            Assert.NotNull(entity.ModifiedAt);
            Assert.Equal(expected.Type, entity.Type);
            Assert.Equal(expected.Subject, entity.Subject);
            Assert.Equal(expected.Body, entity.Body);
        }

        [Fact]
        public void Exists_Should_ReturnTrue_IfEntityExists()
        {
            var exists = _intRepo.Exists(_intList.First().Id);
            Assert.True(exists);
        }

        [Fact]
        public void Exists_Should_ReturnFalse_IfEntityExists()
        {
            var exists = _intRepo.Exists(999);
            Assert.False(exists);
        }

        [Fact]
        public void Create_Should_CreateAndReturnEntity()
        {
            _seeder.ClearMessageTemplates();
            var expectedEntity = new MessageTemplateBuilder()
                .WithId(555)
                .WithType(MessageType.ColidEntrySubscriptionUpdate)
                .WithSubject("subj")
                .WithBody("body")
                .Build();

            var entity = _intRepo.Create(expectedEntity);

            Assert.NotNull(entity);
            Assert.NotNull(entity.CreatedAt);
            Assert.NotNull(entity.ModifiedAt);
            Assert.Equal(expectedEntity.Type, entity.Type);
            Assert.Equal(expectedEntity.Subject, entity.Subject);
            Assert.Equal(expectedEntity.Body, entity.Body);
            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void Create_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.Create(null));
        }

        [Fact]
        public void Update_Should_UpdateAndReturnEntity()
        {
            //var oldEntity = _intRepo.GetOne(3); // bad ..
            
            _seeder.ResetMessageTemplates();
            var oldEntity = TestData.GetPreconfiguredMessageTemplates()
                .FirstOrDefault(x => MessageType.StoredQueryResult.Equals(x.Type));

            var changedEntity = _intRepo.GetOne(oldEntity.Id);
            changedEntity.Type = MessageType.StoredQueryResult;
            changedEntity.Subject = "Updated subject";
            changedEntity.Body = "Updated body";

            var updatedEntity = _intRepo.Update(changedEntity);
            Assert.NotNull(updatedEntity);
            Assert.NotNull(updatedEntity.CreatedAt);
            Assert.NotNull(updatedEntity.ModifiedAt);

            Assert.NotEqual(oldEntity.Subject, changedEntity.Subject);
            Assert.NotEqual(oldEntity.Body, changedEntity.Body);

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void Update_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.Update(null));
        }

        [Fact]
        public void DeleteByEntity_Should_DeleteEntity()
        {
            var entityToDelete = _intRepo.GetOne(2);
            Assert.NotNull(entityToDelete);

            _intRepo.Delete(entityToDelete);

            var exists = _intRepo.Exists(entityToDelete.Id);
            Assert.False(exists);

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void DeleteByEntity_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.Delete(null));
        }

        [Fact]
        public void DeleteRangeByEntities_Should_DeleteEntity()
        {
            var firstEntity = _intRepo.GetOne(1);
            var secondEntity = _intRepo.GetOne(2);

            var entityList = new Collection<MessageTemplate>();
            entityList.Add(firstEntity);
            entityList.Add(secondEntity);
            Assert.NotEmpty(entityList);
            Assert.Equal(2, entityList.Count);

            _intRepo.DeleteRange(entityList);

            Assert.False(_intRepo.Exists(firstEntity.Id));
            Assert.False(_intRepo.Exists(secondEntity.Id));

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void DeleteRangeByEntities_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.DeleteRange(null));
        }

        [Fact]
        public void DeleteRangeByEntities_Should_ThrowException_IfArgumentIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => _intRepo.DeleteRange(new Collection<MessageTemplate>()));
        }

        #endregion [Synchronous tests]

        #region [Asynchron tests]

        [Fact]
        public async Task GetAllAsync_Should_ReturnEntities()
        {
            int amountOfEntities = _intList.Count();
            var entitiesFromRepository = await _intRepo.GetAllAsync();

            Assert.NotNull(entitiesFromRepository);
            Assert.Equal(amountOfEntities, entitiesFromRepository.Count());
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnEmptyContent_IfNoEntitesExist()
        {
            _seeder.ClearMessageTemplates();

            var entitiesFromRepository = await _intRepo.GetAllAsync();
            Assert.NotNull(entitiesFromRepository);
            Assert.Equal(0, entitiesFromRepository.Count());

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public async Task GetOneAsync_Should_ReturnOneEntity()
        {
            var expected = _intList.First();
            var actualEntity = await _intRepo.GetOneAsync(expected.Id);

            Assert.NotNull(actualEntity);
            Assert.Equal(expected.Type, actualEntity.Type);
            Assert.Equal(expected.Subject, actualEntity.Subject);
            Assert.Equal(expected.Body, actualEntity.Body);
        }

        [Fact]
        public void GetOneAsync_Should_ThrowException_IfEntityDoesNotExist()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(() => _intRepo.GetOneAsync(999));
        }

        [Fact]
        public async Task ExistsAsync_Should_ReturnTrue_IfEntityExists()
        {
            var exists = await _intRepo.ExistsAsync(_intList.First().Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_Should_ReturnFalse_IfEntityExists()
        {
            var exists = await _intRepo.ExistsAsync(999);
            Assert.False(exists);
        }

        [Fact]
        public async Task CreateAsync_Should_CreateAndReturnEntity()
        {
            _seeder.ClearMessageTemplates();
            var expectedEntity = new MessageTemplateBuilder()
                .WithId(555)
                .WithType(MessageType.ColidEntrySubscriptionUpdate)
                .WithSubject("subj")
                .WithBody("body")
                .Build();

            var entity = await _intRepo.CreateAsync(expectedEntity);

            Assert.NotNull(entity);
            Assert.NotNull(entity.CreatedAt);
            Assert.NotNull(entity.ModifiedAt);
            Assert.Equal(expectedEntity.Type, entity.Type);
            Assert.Equal(expectedEntity.Subject, entity.Subject);
            Assert.Equal(expectedEntity.Body, entity.Body);
            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public void CreateAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _intRepo.CreateAsync(null));
        }

        #endregion [Asynchron tests]
    }
}
