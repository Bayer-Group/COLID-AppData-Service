using System;
using System.Collections.Generic;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.AppDataService.Tests.Unit.Builder;
using COLID.Cache.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class MessageTemplateServiceTests
    {
        private readonly IMessageTemplateService _templateService;
        private readonly Mock<IMessageTemplateRepository> _mockMessageTemplateRepository = new Mock<IMessageTemplateRepository>();
        private readonly Mock<ILogger<MessageTemplateService>> _mockLogger = new Mock<ILogger<MessageTemplateService>>();

        public MessageTemplateServiceTests()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
            var mapper = new Mapper(configuration);
            var noCache = new NoCacheService();
            _templateService = new MessageTemplateService(_mockMessageTemplateRepository.Object, mapper, noCache, _mockLogger.Object);
        }

        [Fact]
        public void GetOne_Should_InvokeMessageTemplateRepositoryGetOne_Once()
        {
            var messageType = MessageType.ColidEntrySubscriptionUpdate;
            _templateService.GetOne(messageType);
            _mockMessageTemplateRepository.Verify(x => x.GetOne(messageType), Times.Once);
        }

        [Fact]
        public void Create_Should_InvokeMessageTemplateRepositoryCreate_Once()
        {
            var template = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            _templateService.Create(template);

            MessageTemplate _;
            _mockMessageTemplateRepository.Verify(x => x.TryGetOne(It.IsAny<MessageType>(), out _), Times.Once);
            _mockMessageTemplateRepository.Verify(x => x.Create(It.IsAny<MessageTemplate>()), Times.Once);
        }

        [Fact]
        public void Create_Should_ThrowException_IfTemplateAlreadyExists()
        {
            MessageTemplate _;
            _mockMessageTemplateRepository.Setup(x => x.TryGetOne(MessageType.StoredQueryResult, out _)).Returns(true);

            var template = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            Assert.Throws<EntityAlreadyExistsException>(() => _templateService.Create(template));
        }

        [Fact]
        public void CreateAsync_Should_InvokeMessageTemplateRepositoryCreateAsync_Once()
        {
            var template = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            _templateService.CreateAsync(template);

            MessageTemplate _;
            _mockMessageTemplateRepository.Verify(x => x.TryGetOne(It.IsAny<MessageType>(), out _), Times.Once);
            _mockMessageTemplateRepository.Verify(x => x.CreateAsync(It.IsAny<MessageTemplate>()), Times.Once);
        }

        [Fact]
        public void CreateAsync_Should_ThrowException_IfTemplateAlreadyExists()
        {
            MessageTemplate _;
            _mockMessageTemplateRepository.Setup(x => x.TryGetOne(MessageType.StoredQueryResult, out _)).Returns(true);

            var template = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _templateService.CreateAsync(template));
        }

        [Fact]
        public void Update_Should_InvokeMessageTemplateRepositoryUpdate_Once()
        {
            var template = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("bbb").WithBody("ccc").Build();
            _mockMessageTemplateRepository.Setup(x => x.GetOne(MessageType.StoredQueryResult)).Returns(template);

            var templateDto = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            _templateService.Update(templateDto);

            MessageTemplate _;
            _mockMessageTemplateRepository.Verify(x => x.TryGetOne(It.IsAny<MessageTemplateDto>(), out _), Times.Once);
            _mockMessageTemplateRepository.Verify(x => x.GetOne(It.IsAny<MessageType>()), Times.Once);
            _mockMessageTemplateRepository.Verify(x => x.Update(It.IsAny<MessageTemplate>()), Times.Once);
        }

        [Fact]
        public void Update_Should_ThrowException_IfTemplateIsUnchanged()
        {
            MessageTemplate _;
            _mockMessageTemplateRepository.Setup(x => x.TryGetOne(It.IsAny<MessageTemplateDto>(), out _)).Returns(true);

            var templateDto = new MessageTemplateBuilder().WithType(MessageType.StoredQueryResult).WithSubject("aaa").WithBody("bbb").BuildDto();
            Assert.Throws<EntityNotChangedException>(() => _templateService.Update(templateDto));
        }
    }
}
