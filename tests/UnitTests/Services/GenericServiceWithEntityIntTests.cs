using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.Exception.Models.Business;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class GenericServiceWithEntityIntTests
    {
        private readonly Mock<IGenericRepository<Entity<int?>, int?>> _mockGenericRepositoryNullable;
        private readonly GenericService<Entity<int?>, int?> _genericServiceNullable;

        private readonly Mock<IGenericRepository<Entity<int>, int>> _mockGenericRepository;
        private readonly GenericService<Entity<int>, int> _genericService;

        private int _testId = 0;

        private Entity<int> _testEntity = new Entity<int>();

        public GenericServiceWithEntityIntTests()
        {
            _mockGenericRepository = new Mock<IGenericRepository<Entity<int>, int>>();
            _genericService = new GenericService<Entity<int>, int>(_mockGenericRepository.Object);

            // Nullable repo and service to check methods, that doesn't allow null in general
            _mockGenericRepositoryNullable = new Mock<IGenericRepository<Entity<int?>, int?>>();
            _genericServiceNullable = new GenericService<Entity<int?>, int?>(_mockGenericRepositoryNullable.Object);
        }

        #region GetAll tests

        [Fact]
        public void GetAll_Should_InvokeGenericRepositoryGetAll_Once()
        {
            var returnValue = _genericService.GetAll();
            _mockGenericRepository.Verify(x => x.GetAll(It.IsAny<bool>()), Times.Once);
        }

        #endregion GetAll tests

        #region GetOne tests

        [Fact]
        public void GetOne_Should_InvokeGenericRepositoryGetOne_Once()
        {
            var returnValue = _genericService.GetOne(_testId);
            _mockGenericRepository.Verify(x => x.GetOne(_testId, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void GetOne_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericServiceNullable.GetOne(null));
        }

        #endregion GetOne tests

        #region TryGetOne tests

        [Fact]
        public void TryGetOne_Should_InvokeGenericRepositoryTryGetOne_Once()
        {
            var returnValue = _genericService.TryGetOne(_testId, out var outParam);
            _mockGenericRepository.Verify(x => x.TryGetOne(_testId, out outParam, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void TryGetOne_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericServiceNullable.TryGetOne(null, out var outParam));
        }

        #endregion TryGetOne tests

        #region Exists tests

        [Fact]
        public void Exists_Should_InvokeGenericRepositoryExits_Once()
        {
            var returnValue = _genericService.Exists(_testId);
            _mockGenericRepository.Verify(x => x.Exists(_testId), Times.Once);
        }

        [Fact]
        public void Exists_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericServiceNullable.Exists((int?)null));
        }

        #endregion Exists tests

        #region Create tests

        [Fact]
        public void Create_Should_InvokeGenericRepositoryCreate_Once()
        {
            Entity<int> outParam;
            _genericService.Create(_testEntity);
            _mockGenericRepository.Verify(x => x.TryGetOne(_testEntity.Id, out outParam, It.IsAny<bool>()), Times.Once);
            _mockGenericRepository.Verify(x => x.Create(_testEntity), Times.Once);
        }

        [Fact]
        public void Create_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericService.Create(null));
        }

        [Fact]
        public void Create_Should_ThrowException_IfEntityAlreadyExists()
        {
            Entity<int> outParam;
            _mockGenericRepository
                .Setup(x => x.TryGetOne(It.IsAny<int>(), out outParam, It.IsAny<bool>()))
                .Returns(true);
            Assert.Throws<EntityAlreadyExistsException>(() => _genericService.Create(_testEntity));
        }

        #endregion Create tests

        #region Update tests

        [Fact]
        public void Update_Should_InvokeGenericRepositoryUpdate_Once()
        {
            _mockGenericRepository
                .Setup(x => x.Exists(It.IsAny<int>()))
                .Returns(true);

            _genericService.Update(_testEntity);

            _mockGenericRepository.Verify(x => x.Exists(_testEntity.Id), Times.Once);
            _mockGenericRepository.Verify(x => x.Update(_testEntity), Times.Once);
        }

        [Fact]
        public void Update_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericService.Update(null));
        }

        [Fact]
        public void Update_Should_ThrowException_IfEntityNotExists()
        {
            Assert.Throws<EntityNotFoundException>(() => _genericService.Update(_testEntity));
        }

        #endregion Update tests

        #region Delete tests

        [Fact]
        public void DeleteByEntity_Should_InvokeGenericRepositoryDeleteByEntity_Once()
        {
            _genericService.Delete(_testEntity);
            _mockGenericRepository.Verify(x => x.Delete(_testEntity), Times.Once);
        }

        [Fact]
        public void DeleteById_Should_InvokeGenericRepositoryDeleteById_Once()
        {
            _genericService.Delete(_testId);
            _mockGenericRepository.Verify(x => x.Delete(It.IsAny<Entity<int>>()), Times.Once);
        }

        [Fact]
        public void DeleteByEntity_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericService.Delete(null));
        }

        [Fact]
        public void DeleteById_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericServiceNullable.Delete((int?)null));
        }

        #endregion Delete tests

        #region DeleteRange tests

        [Fact]
        public void DeleteRangeByEntities_Should_InvokeGenericRepositoryDeleteRange_Once()
        {
            var entityList = new Collection<Entity<int>>() { new Entity<int>(), new Entity<int>(), new Entity<int>() };
            _genericService.DeleteRange(entityList);
            _mockGenericRepository.Verify(x => x.DeleteRange(entityList), Times.Once);
        }

        [Fact]
        public void DeleteRangeByEntities_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _genericService.DeleteRange(null));
        }

        #endregion DeleteRange tests

        #region GetAllAsync tests

        [Fact]
        public void GetAllAsync_Should_InvokeGenericRepositoryGetAllAsync_Once()
        {
            var returnValue = _genericService.GetAllAsync();
            _mockGenericRepository.Verify(x => x.GetAllAsync(It.IsAny<bool>()), Times.Once);
        }

        #endregion GetAllAsync tests

        #region GetOneAsync tests

        [Fact]
        public void GetOneAsync_Should_InvokeGenericRepositoryGetOneAsync_Once()
        {
            var returnValue = _genericService.GetOneAsync(_testId);
            _mockGenericRepository.Verify(x => x.GetOneAsync(_testId, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void GetOneAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _genericServiceNullable.GetOneAsync((int?)null));
        }

        #endregion GetOneAsync tests

        #region ExistsAsync tests

        [Fact]
        public void ExistsAsync_Should_InvokeGenericRepositoryExitsAsync_Once()
        {
            var returnValue = _genericService.ExistsAsync(_testId);
            _mockGenericRepository.Verify(x => x.ExistsAsync(_testId), Times.Once);
        }

        [Fact]
        public void ExistsAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _genericServiceNullable.ExistsAsync((int?)null));
        }

        #endregion ExistsAsync tests

        #region CreateAsync tests

        [Fact]
        public async Task CreateAsync_Should_InvokeGenericRepositoryCreate_Once()
        {
            Entity<int> outParam;
            await _genericService.CreateAsync(_testEntity);
            _mockGenericRepository.Verify(x => x.TryGetOne(_testEntity.Id, out outParam, It.IsAny<bool>()), Times.Once);
            _mockGenericRepository.Verify(x => x.CreateAsync(_testEntity), Times.Once);
        }

        [Fact]
        public void CreateAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _genericService.CreateAsync(null));
        }

        [Fact]
        public void CreateAsync_Should_ThrowException_IfEntityAlreadyExists()
        {
            Entity<int> outParam;
            _mockGenericRepository
                .Setup(x => x.TryGetOne(It.IsAny<int>(), out outParam, It.IsAny<bool>()))
                .Returns(true);
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _genericService.CreateAsync(_testEntity));
        }

        #endregion CreateAsync tests
    }
}
