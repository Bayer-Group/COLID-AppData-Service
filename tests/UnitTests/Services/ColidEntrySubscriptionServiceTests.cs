using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.AppDataService.Tests.Unit.Builder;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class ColidEntrySubscriptionServiceTests
    {
        private readonly IColidEntrySubscriptionService _subscriptionService;
        private readonly Mock<IColidEntrySubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<ILogger<ColidEntrySubscriptionService>> _mockLogger;

        private readonly Guid _userId;
        private readonly ColidEntrySubscription _colidEntrySubscription;
        private readonly ColidEntrySubscriptionDto _colidEntrySubscriptionDto;

        public ColidEntrySubscriptionServiceTests()
        {
            _mockSubscriptionRepository = new Mock<IColidEntrySubscriptionRepository>();
            _mockLogger = new Mock<ILogger<ColidEntrySubscriptionService>>();
            _subscriptionService = new ColidEntrySubscriptionService(_mockSubscriptionRepository.Object, _mockLogger.Object);

            // Init testdata
            _userId = Guid.NewGuid();
            var builder = new ColidEntrySubscriptionBuilder().WithColidEntry(new Uri($"https://pid.bayer.com/kos19050#{Guid.NewGuid()}"));
            _colidEntrySubscription = builder.Build();
            _colidEntrySubscriptionDto = builder.BuildDto();

            // Init mock behaviour
            _mockSubscriptionRepository.Setup(x => x.GetOne(It.IsAny<Guid>(), It.IsAny<Uri>()))
                .Returns(_colidEntrySubscription);

            IList<User> subscribedUsers;
            _mockSubscriptionRepository.Setup(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers));
        }

        [Fact]
        public void GetOneByUserIdAndDto_Should_InvokeColidEntrySubscriptionRepositoryGetOne_Once()
        {
            _subscriptionService.GetOne(_userId, _colidEntrySubscriptionDto);
            _mockSubscriptionRepository.Verify(x => x.GetOne(_userId, _colidEntrySubscriptionDto.ColidPidUri), Times.Once);
        }

        [Fact]
        public void GetOneByUserIdAndDto_Should_ThrowException_IfDtoIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _subscriptionService.GetOne(Guid.NewGuid(), null));
        }

        [Fact]
        public void TryGetAllUsers_Should_InvokeColidEntrySubscriptionRepositoryTryGetAllUsers_Once()
        {
            var returnValue = _subscriptionService.TryGetAllUsers(_colidEntrySubscriptionDto.ColidPidUri, out var outParam);
            _mockSubscriptionRepository.Verify(x => x.TryGetAllUsers(_colidEntrySubscriptionDto.ColidPidUri, out outParam), Times.Once);
        }

        [Fact]
        public void TryGetAllUsers_Should_ThrowException_IfUriIsInvalid()
        {
            Assert.Throws<UriFormatException>(() => _subscriptionService.TryGetAllUsers(new Uri("asdnlas"), out var outParam));
        }

        [Fact]
        public void Delete_Should_InvokeColidEntrySubscriptionRepositoryDelete_Once()
        {
            var dto = new ColidEntrySubscriptionDto() {ColidPidUri = new Uri("http://www.this.com/is_a_special_meh!")};
            _subscriptionService.Delete(dto);
            _mockSubscriptionRepository.Verify(x => x.Delete(dto), Times.Once);
        }

        [Fact]
        public void Delete_Should_ThrowException_IfDtoIsNull()
        {
            ColidEntrySubscriptionDto nullDto = null;
            Assert.Throws<ArgumentNullException>(() => _subscriptionService.Delete(nullDto));
        }

        [Fact]
        public void GetColidPidUrisAndAmountSubscriptions_Should_InvokeColidEntrySubscriptionRepositoryCountSubsciptionsPerColidPidUri_Once()
        {
            _subscriptionService.GetColidPidUrisAndAmountSubscriptions();
            _mockSubscriptionRepository.Verify(x => x.CountSubsciptionsPerColidPidUri(), Times.Once);
        }
    }
}
