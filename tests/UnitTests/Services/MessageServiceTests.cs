using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.AppDataService.Tests.Unit.Builder;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class MessageServiceTests
    {
        private readonly IMessageService _messageService;

        private readonly Mock<IMessageRepository> _mockMessageRepository = new Mock<IMessageRepository>();
        private readonly Mock<IUserService> _mockUserService = new Mock<IUserService>();
        private readonly Mock<IColidEntrySubscriptionService> _mockColidEntrySubscriptionService = new Mock<IColidEntrySubscriptionService>();
        private readonly Mock<IMessageTemplateService> _mockMessageTemplateService = new Mock<IMessageTemplateService>();
        private readonly Mock<IActiveDirectoryService> _mockActiveDirectoryService = new Mock<IActiveDirectoryService>();
        private readonly Mock<ILogger<MessageService>> _mockLogger = new Mock<ILogger<MessageService>>();

        public MessageServiceTests()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
            var mapper = new Mapper(configuration);
            _messageService = new MessageService(_mockMessageRepository.Object, _mockUserService.Object, _mockMessageTemplateService.Object,
                _mockColidEntrySubscriptionService.Object, _mockActiveDirectoryService.Object, mapper, _mockLogger.Object);
        }

        #region CreateUpdateMessagesForColidEntrySubscriptions

        [Fact] // subscription field within the user won't be checked here, because it's part of another service
        public async void CreateMessagesUpdateForColidEntrySubscriptions_Should_CreateMessagesForSubscribedUsers()
        {
            // ARRANGE
            IList<User> subscribedUsers = new List<User>() { TestData.GenerateRandomUser(), TestData.GenerateRandomUser(), TestData.GenerateRandomUser() };
            _mockColidEntrySubscriptionService.Setup(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers)).Returns(true);

            var template = TestData.GenerateSampleMessageTemplateForColidEntrySubscriptionUpdate();
            _mockMessageTemplateService.Setup(x => x.GetOne(MessageType.ColidEntrySubscriptionUpdate)).Returns(template);

            var messageDtoList = new List<MessageDto>(); // To check the generated MessageDto
            _mockUserService.Setup(x => x.AddMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageDto>()))
                .Callback<Guid, MessageDto>((userId, messageDto) => messageDtoList.Add(messageDto));

            var messageConfig = TestData.GenerateSampleMessageConfig();
            _mockUserService.Setup(x => x.GetMessageConfigAsync(It.IsAny<Guid>())).ReturnsAsync(messageConfig);

            // ACT
            var colidEntryDto = new ColidEntryDtoBuilder().WithLabel("ALMIGHTY RESOURCE").WithColidPidUri("http://meh").Build();
            var result = await _messageService.CreateUpdateMessagesForColidEntrySubscriptions(colidEntryDto);

            // ASSERT
            Assert.Equal(3, result);
            Assert.NotNull(messageDtoList);

            var msg = messageDtoList.First();
            Assert.DoesNotContain("%COLID_LABEL%", msg.Subject);
            Assert.DoesNotContain("%COLID_PID_URI%", msg.Subject);
            Assert.DoesNotContain("%COLID_LABEL%", msg.Body);
            Assert.DoesNotContain("%COLID_PID_URI%", msg.Body);
            Assert.Null(msg.ReadOn);
            Assert.NotNull(msg.SendOn);
            Assert.NotNull(msg.DeleteOn);

            _mockColidEntrySubscriptionService.Verify(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers), Times.Once);
            _mockMessageTemplateService.Verify(x => x.GetOne(MessageType.ColidEntrySubscriptionUpdate), Times.Once);
            _mockUserService.Verify(x => x.AddMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageDto>()), Times.Exactly(3));
        }

        [Fact]
        public void CreateUpdateMessagesForColidEntrySubscriptions_Should_ThrowException_IfArgumentIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _messageService.CreateUpdateMessagesForColidEntrySubscriptions(new ColidEntryDto()));
        }

        [Fact]
        public void CreateUpdateMessagesForColidEntrySubscriptions_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _messageService.CreateUpdateMessagesForColidEntrySubscriptions(null));
        }

        [Fact]
        public async Task CreateUpdateMessagesForColidEntrySubscriptions_Should_Return0_IfNoUserSubscribedToTheColidPidUri()
        {
            IList<User> subscribedUsers;
            _mockColidEntrySubscriptionService.Setup(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers)).Returns(false);

            var colidEntryDto = new ColidEntryDtoBuilder().WithLabel("ALMIGHTY RESOURCE").WithColidPidUri("http://meh").Build();
            var result = await _messageService.CreateUpdateMessagesForColidEntrySubscriptions(colidEntryDto);
            Assert.Equal(0, result);
        }

        #endregion

        #region CreateDeleteMessagesAndRemoveColidEntrySubscriptions

        [Fact] // subscription field within the user won't be checked here, because it's part of another service
        public async void CreateDeleteMessagesAndRemoveColidEntrySubscriptions_Should_CreateMessagesForSubscribedUsers()
        {
            // ARRANGE
            IList<User> subscribedUsers = new List<User>() { TestData.GenerateRandomUser(), TestData.GenerateRandomUser(), TestData.GenerateRandomUser() };
            _mockColidEntrySubscriptionService.Setup(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers)).Returns(true);

            var template = TestData.GenerateSampleMessageTemplateForColidEntrySubscriptionDelete();
            _mockMessageTemplateService.Setup(x => x.GetOne(MessageType.ColidEntrySubscriptionDelete)).Returns(template);

            var messageDtoList = new List<MessageDto>(); // To check the generated MessageDto
            _mockUserService.Setup(x => x.AddMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageDto>()))
                .Callback<Guid, MessageDto>((userId, messageDto) => messageDtoList.Add(messageDto));

            var messageConfig = TestData.GenerateSampleMessageConfig();
            _mockUserService.Setup(x => x.GetMessageConfigAsync(It.IsAny<Guid>())).ReturnsAsync(messageConfig);

            // ACT
            var colidEntryDto = new ColidEntryDtoBuilder().WithLabel("ALMIGHTY RESOURCE").WithColidPidUri("http://meh").Build();
            var result = await _messageService.CreateDeleteMessagesAndRemoveColidEntrySubscriptions(colidEntryDto);

            // ASSERT
            Assert.Equal(3, result);
            Assert.NotNull(messageDtoList);

            var msg = messageDtoList.First();
            Assert.DoesNotContain("%COLID_LABEL%", msg.Subject);
            Assert.DoesNotContain("%COLID_PID_URI%", msg.Subject);
            Assert.DoesNotContain("%COLID_LABEL%", msg.Body);
            Assert.DoesNotContain("%COLID_PID_URI%", msg.Body);
            Assert.Null(msg.ReadOn);
            Assert.NotNull(msg.SendOn);
            Assert.NotNull(msg.DeleteOn);

            _mockColidEntrySubscriptionService.Verify(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers), Times.Once);
            _mockMessageTemplateService.Verify(x => x.GetOne(MessageType.ColidEntrySubscriptionDelete), Times.Once);
            _mockUserService.Verify(x => x.AddMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageDto>()), Times.Exactly(3));
            _mockColidEntrySubscriptionService.Verify(x => x.Delete(It.IsAny<ColidEntrySubscriptionDto>()));
        }

        [Fact]
        public void CreateDeleteMessagesAndRemoveColidEntrySubscriptions_Should_ThrowException_IfArgumentIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _messageService.CreateDeleteMessagesAndRemoveColidEntrySubscriptions(new ColidEntryDto()));
        }

        [Fact]
        public void CreateDeleteMessagesAndRemoveColidEntrySubscriptions_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _messageService.CreateDeleteMessagesAndRemoveColidEntrySubscriptions(null));
        }

        [Fact]
        public async Task CreateDeleteMessagesAndRemoveColidEntrySubscriptions_Should_Return0_IfNoUserSubscribedToTheColidPidUri()
        {
            IList<User> subscribedUsers;
            _mockColidEntrySubscriptionService.Setup(x => x.TryGetAllUsers(It.IsAny<Uri>(), out subscribedUsers)).Returns(false);

            var colidEntryDto = new ColidEntryDtoBuilder().WithLabel("ALMIGHTY RESOURCE").WithColidPidUri("http://meh").Build();
            var result = await _messageService.CreateDeleteMessagesAndRemoveColidEntrySubscriptions(colidEntryDto);
            Assert.Equal(0, result);
        }

        #endregion


        [Fact]
        public void GetUnreadMessagesToSend_Should_InvokeMessageRepositoryGetUnreadMessagesToSend_Once()
        {
            var user = TestData.GenerateRandomUser();
            var msg1 = new MessageBuilder().WithSubject("one").WithBody("two").WithUser(user).WithSendOn(DateTime.Now.AddDays(-2)).Build();
            var msg2 = new MessageBuilder().WithSubject("two").WithBody("thre").WithUser(user).WithSendOn(DateTime.Now.AddMinutes(-5)).Build();
            var messageList = new List<Message>() { msg1, msg2 };
            _mockMessageRepository.Setup(x => x.GetUnreadMessagesToSend()).Returns(messageList);

            var unreadMessages = _messageService.GetUnreadMessagesToSend();

            _mockMessageRepository.Verify(x => x.GetUnreadMessagesToSend(), Times.Once);

            // Due to the mapping logic, this will be checked here also
            var actualMsg1 = unreadMessages.FirstOrDefault(m => m.Subject.Contains("one"));
            Assert.NotNull(actualMsg1);
            Assert.Equal(msg1.Subject, actualMsg1.Subject);
            Assert.Equal(msg1.Body, actualMsg1.Body);
            Assert.Equal(msg1.SendOn, actualMsg1.SendOn);
            Assert.Null(actualMsg1.ReadOn);
            Assert.Null(actualMsg1.ReadOn);
            Assert.Equal(user.Id, actualMsg1.UserId);
            Assert.Equal(user.EmailAddress, actualMsg1.UserEmail);
        }
    }
}
