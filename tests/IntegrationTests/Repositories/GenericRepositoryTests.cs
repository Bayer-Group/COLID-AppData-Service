using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Tests.Unit;
using COLID.Exception.Models.Business;
using Xunit;
using Xunit.Abstractions;
using User = COLID.AppDataService.Common.DataModel.User;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class GenericRepositoryTest : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IGenericRepository _repo;
        private readonly ICollection<User> _users = TestData.GetPreconfiguredUsers().ToImmutableList();

        public GenericRepositoryTest(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new GenericRepository(_context);
            _context.Database.EnsureCreated();

            _seeder.SeedUsers(); // use Users, because they are the main part of this application
        }

        //#region [Synchronous tests]

        [Fact]
        public void GetAll_Should_Return_EntityList()
        {
            var result = _repo.GetAll<User>();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_EntityList()
        {
            var result = await _repo.GetAllAsync<User>();
            Assert.NotNull(result);
        }

        [Fact]
        public void Get_Should_Return_Entity()
        {
            var result = _repo.GetEntities<User>();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Entity()
        {
            var result = await _repo.GetAsync<User>();
            Assert.NotNull(result);
        }

        [Fact]
        public void GetOne_Should_Return_OneEntity()
        {
            var userId = _users.First().Id;
            var result = _repo.GetOne<User>(u => u.Id.Equals(userId));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetOneAsync_Should_Return_OneEntity()
        {
            var userId = _users.First().Id;
            var result = await _repo.GetOneAsync<User>(u => u.Id.Equals(userId));
            Assert.NotNull(result);
        }

        [Fact]
        public void TryGetOneAsync_Should_Return_True_IfEntityExists()
        {
            var userId = _users.First().Id;
            var result = _repo.TryGetOne<User>(out var user, u => u.Id.Equals(userId));
            Assert.True(result);
            Assert.NotNull(user);
        }

        [Fact]
        public void TryGetOneAsync_Should_Return_False_IfEntityNotExists()
        {
            var invalidUserId = Guid.NewGuid();
            var result = _repo.TryGetOne<User>(out var user, u => u.Id.Equals(invalidUserId));
            Assert.False(result);
            Assert.Null(user);
        }

        [Fact]
        public void GetById_Should_Return_EntityById()
        {
            var userId = _users.First().Id;
            var result = _repo.GetById<User>(userId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_EntityById()
        {
            var userId = _users.First().Id;
            var result = await _repo.GetByIdAsync<User>(userId);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetCount_Should_Return_AmountOfEntities()
        {
            var amount = _users.Count;
            var result = _repo.GetCount<User>();
            Assert.Equal(amount, result);
        }

        [Fact]
        public async Task GetCountAsync_Should_Return_AmountOfEntities()
        {
            var amount = _users.Count;
            var result = await _repo.GetCountAsync<User>();
            Assert.Equal(amount, result);
        }

        [Fact]
        public void GetExists_Should_Return_True_IfEntityExists()
        {
            var userId = _users.First().Id;
            var result = _repo.GetExists<User>(u => u.Id.Equals(userId));
            Assert.True(result);
        }

        [Fact]
        public async Task GetExistsAsync_Should_Return_True_IfEntityExists()
        {
            var userId = _users.First().Id;
            var result = await _repo.GetExistsAsync<User>(u => u.Id.Equals(userId));
            Assert.True(result);
        }

        [Fact]
        public void Create_Should_Create_Entity()
        {
            var user = new User { Id = new Guid(), EmailAddress = "chris.kaubisch@greyblack.de" };
            _repo.Create(user);

            _seeder.ResetUsers();
        }

        [Fact]
        public void Update_Should_Update_Entity()
        {
            var user = _users.First();
            var userEntity = _repo.GetOne<User>(u => u.Id.Equals(user.Id));

            userEntity.EmailAddress = "peter.lustig@test.com";
            _repo.Update(userEntity);
            _repo.Save();

            var updatedUserEntity = _repo.GetOne<User>(u => u.Id.Equals(user.Id));
            Assert.Equal(userEntity.EmailAddress, updatedUserEntity.EmailAddress);
            Assert.NotEqual(user.EmailAddress, updatedUserEntity.EmailAddress);

            _seeder.ResetUsers();
        }

        [Fact]
        public void DeleteById_Should_Delete_Entity()
        {
            var user = _users.First();
            var userExistInDb = _repo.GetExists<User>(u => u.Id.Equals(user.Id));
            Assert.True(userExistInDb);

            _repo.Delete<User>(user.Id);
            _repo.Save();

            Assert.Throws<EntityNotFoundException>(() => _repo.GetOne<User>(u => u.Id.Equals(user.Id)));

            _seeder.ResetUsers();
        }

        [Fact]
        public void DeleteByEntity_Should_Delete_Entity()
        {
            var user = _users.First();
            var userExistInDb = _repo.GetExists<User>(u => u.Id.Equals(user.Id));
            Assert.True(userExistInDb);

            _repo.Delete(user);
            _repo.Save();

            Assert.Throws<EntityNotFoundException>(() => _repo.GetOne<User>(u => u.Id.Equals(user.Id)));

            _seeder.ResetUsers();
        }

        [Fact]
        public void DeleteRange_Should_Delete_MultipleEntities()
        {
            var users = _users;
            Assert.True(users.Count > 1);

            var userEntities = _repo.GetAll<User>();

            _repo.DeleteRange(userEntities);
            _repo.Save();

            var userEntitiesAfterDelete = _repo.GetAll<User>();
            Assert.Empty(userEntitiesAfterDelete);

            _seeder.ResetUsers();
        }

        /*
        [Fact]
        public void FindByCondition_Should_ReturnQueryableObject()
        {
            Assert.NotNull(_repo.FindByCondition(e => e.IsNotEmpty()));
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

        */
    }
}
