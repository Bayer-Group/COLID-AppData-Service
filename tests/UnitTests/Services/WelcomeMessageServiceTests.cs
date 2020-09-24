using System;
using System.Linq;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.Cache.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class WelcomeMessageServiceTests
    {
        private readonly IWelcomeMessageService _welcomeMessageService;
        private readonly ICacheService _cache;
        private readonly Mock<IWelcomeMessageRepository> _mockWelcomeMessageRepository;
        private readonly WelcomeMessage _welcomeMessage;

        public WelcomeMessageServiceTests()
        {
            _mockWelcomeMessageRepository = new Mock<IWelcomeMessageRepository>();
            _cache = new NoCacheService();
            _welcomeMessageService = new WelcomeMessageService(_mockWelcomeMessageRepository.Object, _cache);

            // Init testdata
            _welcomeMessage = TestData.GetPreconfiguredWelcomeMessages().FirstOrDefault();

            // Init mock behaviour
            _mockWelcomeMessageRepository.Setup(x => x.GetOne(It.IsAny<ColidType>()))
                .Returns(_welcomeMessage);
        }

        [Fact]
        public void GetWelcomeMessageEditor_Should_InvokeConsumerGroupRepositoryGetOne_Once()
        {
            _welcomeMessageService.GetWelcomeMessageEditor();
            _mockWelcomeMessageRepository.Verify(x => x.GetOne(ColidType.Editor), Times.Once);
        }

        [Fact]
        public void GetWelcomeMessageDataMarketplace_Should_InvokeConsumerGroupRepositoryGetOne_Once()
        {
            _welcomeMessageService.GetWelcomeMessageDataMarketplace();
            _mockWelcomeMessageRepository.Verify(x => x.GetOne(ColidType.DataMarketplace), Times.Once);
        }

        [Fact]
        public void UpdateWelcomeMessageEditor_Should_ReturnUpdatedWelcomeMessageEditor()
        {
            var content = "123";
            _welcomeMessageService.UpdateWelcomeMessageEditor(content);

            _mockWelcomeMessageRepository.Verify(x => x.GetOne(It.IsAny<ColidType>()), Times.Once);
            _mockWelcomeMessageRepository.Verify(x => x.Update(It.IsAny<WelcomeMessage>()), Times.Once);
        }

        [Fact]
        public void UpdateWelcomeMessageEditor_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _welcomeMessageService.UpdateWelcomeMessageEditor(null));
            _mockWelcomeMessageRepository.Verify(x => x.Update(It.IsAny<WelcomeMessage>()), Times.Never);
        }

        [Fact]
        public void UpdateWelcomeMessageDataMarketplace_Should_ReturnUpdatedWelcomeMessageDataMarketplace()
        {
            var content = "123";
            _welcomeMessageService.UpdateWelcomeMessageDataMarketplace(content);

            _mockWelcomeMessageRepository.Verify(x => x.GetOne(It.IsAny<ColidType>()), Times.Once);
            _mockWelcomeMessageRepository.Verify(x => x.Update(It.IsAny<WelcomeMessage>()), Times.Once);
        }

        [Fact]
        public void UpdateWelcomeMessageDataMarketplace_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _welcomeMessageService.UpdateWelcomeMessageDataMarketplace(null));
            _mockWelcomeMessageRepository.Verify(x => x.Update(It.IsAny<WelcomeMessage>()), Times.Never);
        }
    }
}
