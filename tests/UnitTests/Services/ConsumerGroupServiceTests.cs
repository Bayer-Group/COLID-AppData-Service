using System;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.AppDataService.Tests.Unit.Builder;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class ConsumerGroupServiceTests
    {
        private readonly IConsumerGroupService _consumerGroupService;
        private readonly Mock<IConsumerGroupRepository> _mockConsumerGroupRepository;
        private readonly Mock<ILogger<ConsumerGroupService>> _mockLogger;

        private readonly ConsumerGroup _consumerGroup;
        private readonly ConsumerGroupDto _consumerGroupDto;

        public ConsumerGroupServiceTests()
        {
            _mockConsumerGroupRepository = new Mock<IConsumerGroupRepository>();
            _mockLogger = new Mock<ILogger<ConsumerGroupService>>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
            var mapper = new Mapper(configuration);

            _consumerGroupService = new ConsumerGroupService(_mockConsumerGroupRepository.Object, mapper, _mockLogger.Object);

            // Init testdata
            _consumerGroup = new ConsumerGroupBuilder()
                .WithUri(new Uri("https://pid.bayer.com/kos19050#87654321-4321-4321-210987654321"))
                .Build();

            _consumerGroupDto = new ConsumerGroupBuilder()
                .WithUri(new Uri("https://pid.bayer.com/kos19050#12345678-1234-1234-123456789012"))
                .BuildDto();

            // Init mock behaviour
            _mockConsumerGroupRepository.Setup(x => x.GetOne(It.IsAny<Uri>()))
                .Returns(_consumerGroup);
            _mockConsumerGroupRepository.Setup(x => x.Delete(It.IsAny<ConsumerGroup>()));
        }

        [Fact]
        public void GetOneByUri_Should_InvokeConsumerGroupRepositoryGetOne_Once()
        {
            _consumerGroupService.GetOne(_consumerGroupDto);
            _mockConsumerGroupRepository.Verify(x => x.GetOne(_consumerGroupDto.Uri), Times.Once);
        }

        [Fact]
        public void GetOneByUri_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _consumerGroupService.GetOne((Uri)null));
        }

        [Fact]
        public void GetOneByUri_Should_ThrowException_IfUriIsInvalid()
        {
            Assert.Throws<UriFormatException>(() => _consumerGroupService.GetOne(new Uri("asdnlas")));
        }

        [Fact]
        public void TryGetOneByUri_Should_InvokeConsumerGroupRepositoryTryGetOne_Once()
        {
            var returnValue = _consumerGroupService.TryGetOne(_consumerGroupDto, out var outParam);
            _mockConsumerGroupRepository.Verify(x => x.TryGetOne(_consumerGroupDto.Uri, out outParam), Times.Once);
        }

        [Fact]
        public void TryGetOneByUri_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _consumerGroupService.TryGetOne(null, out var outParam));
        }

        [Fact]
        public void CreateByUri_Should_InvokeConsumerGroupRepositoryCreate_Once()
        {
            ConsumerGroup outParam;
            _consumerGroupService.Create(_consumerGroupDto);
            _mockConsumerGroupRepository.Verify(x => x.TryGetOne(_consumerGroupDto.Uri, out outParam), Times.Once);
            _mockConsumerGroupRepository.Verify(x => x.Create(It.IsAny<ConsumerGroup>()), Times.Once);
        }

        [Fact]
        public void CreateByUri_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _consumerGroupService.Create(null));
        }

        [Fact]
        public void CreateByUri_Should_ThrowException_IfEntityAlreadyExists()
        {
            ConsumerGroup outParam;
            _mockConsumerGroupRepository
                .Setup(x => x.TryGetOne(It.IsAny<Uri>(), out outParam))
                .Returns(true);
            Assert.Throws<EntityAlreadyExistsException>(() => _consumerGroupService.Create(_consumerGroupDto));
        }

        [Fact]
        public void CreateByUriAsync_Should_InvokeConsumerGroupRepositoryCreateAsync_Once()
        {
            ConsumerGroup outParam;
            _consumerGroupService.CreateAsync(_consumerGroupDto);
            _mockConsumerGroupRepository.Verify(x => x.TryGetOne(_consumerGroupDto.Uri, out outParam), Times.Once);
            _mockConsumerGroupRepository.Verify(x => x.CreateAsync(It.IsAny<ConsumerGroup>()), Times.Once);
        }

        [Fact]
        public void CreateByUriAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _consumerGroupService.CreateAsync(null));
        }

        [Fact]
        public void CreateByUriAsync_Should_ThrowException_IfEntityAlreadyExists()
        {
            ConsumerGroup outParam;
            _mockConsumerGroupRepository
                .Setup(x => x.TryGetOne(It.IsAny<Uri>(), out outParam))
                .Returns(true);
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _consumerGroupService.CreateAsync(_consumerGroupDto));
        }

        [Fact]
        public void DeleteByUri_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _consumerGroupService.Delete(null));
            _mockConsumerGroupRepository.Verify(x => x.Delete(It.IsAny<ConsumerGroup>()), Times.Never);
        }

        [Fact]
        public void DeleteByUri_Should_ThrowException_IfUriDoesNotExist()
        {
            var nonExistingUri = new Uri("http://non.exising.uri");
            var dto = new ConsumerGroupBuilder().WithUri(nonExistingUri).BuildDto();
            _mockConsumerGroupRepository.Setup(x => x.GetOne(dto.Uri))
                .Throws(new EntityNotFoundException());

            Assert.Throws<EntityNotFoundException>(() => _consumerGroupService.Delete(dto));
            _mockConsumerGroupRepository.Verify(x => x.Delete(It.IsAny<ConsumerGroup>()), Times.Never);
        }
    }
}
