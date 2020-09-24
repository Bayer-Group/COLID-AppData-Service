using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
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
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConsumerGroupService> _mockConsumerGroupService;
        private readonly Mock<IGenericService<SearchFilterDataMarketplace, int>> _mockSearchFilterDataMarketplaceService;
        private readonly Mock<IColidEntrySubscriptionService> _mockColidEntrySubscriptionService;
        private readonly Mock<IGenericService<StoredQuery, int>> _mockStoredQueryService;
        private readonly Mock<ILogger<UserService>> _mockLogger;

        private readonly User _user;
        private readonly ConsumerGroup _consumerGroup;
        private readonly ColidEntrySubscription _colidEntrySubscription;

        private const string JsonString = "{}";

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConsumerGroupService = new Mock<IConsumerGroupService>();
            _mockSearchFilterDataMarketplaceService = new Mock<IGenericService<SearchFilterDataMarketplace, int>>();
            _mockColidEntrySubscriptionService = new Mock<IColidEntrySubscriptionService>();
            _mockStoredQueryService = new Mock<IGenericService<StoredQuery, int>>();
            _mockLogger = new Mock<ILogger<UserService>>();

            // Init test data

            _consumerGroup = new ConsumerGroupBuilder()
               .WithUri(new Uri("http://meh"))
               .Build();

            _colidEntrySubscription = new ColidEntrySubscriptionBuilder()
                .WithColidEntry("http://www.averyhandsomeurl.com/")
                .Build();

            _user = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithEmailAddress("cletus.ewing@grandtheftauto5.com")
                .WithDefaultConsumerGroup(_consumerGroup)
                .WithMessageConfig(new MessageConfig
                {
                    SendInterval = SendInterval.Weekly,
                    DeleteInterval = DeleteInterval.Monthly
                })
                .WithColidEntrySubscriptions(new List<ColidEntrySubscription>() { _colidEntrySubscription })
                .Build();

            // Init mock behaviour

            _mockUserRepository.Setup(x => x.GetOne(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(_user);

            _mockUserRepository.Setup(x => x.GetOneAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(_user);

            _mockConsumerGroupService.Setup(x => x.GetOne(It.IsAny<Uri>()))
                .Returns(_consumerGroup);

            _mockColidEntrySubscriptionService.Setup(x => x.GetOne(It.IsAny<Guid>(), It.IsAny<ColidEntrySubscriptionDto>()))
                .Returns(_colidEntrySubscription);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
            var mapper = new Mapper(configuration);
            _userService = new UserService(_mockUserRepository.Object,
                _mockConsumerGroupService.Object,
                _mockSearchFilterDataMarketplaceService.Object,
                _mockColidEntrySubscriptionService.Object,
                _mockStoredQueryService.Object,
                mapper, _mockLogger.Object);
        }

        #region Create tests

        [Fact]
        public void CreateUserByDto_Should_InvokeUserServiceCreate_Once()
        {
            var userDto = new UserBuilder().WithId(Guid.NewGuid()).WithEmailAddress("joe.exotic@tiger.king").BuildDto();
            _userService.Create(userDto);

            User outParam;

            _mockUserRepository.Verify(x => x.TryGetOne(userDto.Id, out outParam, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void CreateUserByDto_Should_ThrowException_IfEmailIsInvalid()
        {
            var userDto = new UserBuilder().WithId(Guid.NewGuid()).WithEmailAddress("wrongemail").BuildDto();
            Assert.Throws<ArgumentException>(() => _userService.Create(userDto));
        }

        [Fact]
        public void CreateUser_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userService.Create(null));
        }

        [Fact]
        public void CreateUser_Should_ThrowException_IfEntityAlreadyExists()
        {
            User outParam;
            _mockUserRepository
                .Setup(x => x.TryGetOne(It.IsAny<Guid>(), out outParam, It.IsAny<bool>()))
                .Returns(true);

            var userDto = new UserBuilder().WithId(Guid.NewGuid()).WithEmailAddress("joe.exotic@tiger.king").BuildDto();

            Assert.Throws<EntityAlreadyExistsException>(() => _userService.Create(userDto));
        }

        #endregion Create tests

        #region CreateAsync tests

        [Fact]
        public async void CreateUserAsyncByDto_Should_InvokeUserServiceCreateAsync_Once()
        {
            var userDto = new UserBuilder().WithId(Guid.NewGuid()).WithEmailAddress("joe.exotic@tiger.king").BuildDto();

            await _userService.CreateAsync(userDto);
            User outParam;

            _mockUserRepository.Verify(x => x.TryGetOne(userDto.Id, out outParam, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void CreateUserAsyncByDto_Should_ThrowException_IfEmailIsInvalid()
        {
            var userDto = new UserBuilder().WithId(Guid.NewGuid()).WithEmailAddress("wrongemail").BuildDto();
            Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateAsync(userDto));
        }

        [Fact]
        public void CreateUserAsync_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateAsync(null));
        }

        [Fact]
        public void CreateUserAsync_Should_ThrowException_IfEntityAlreadyExists()
        {
            User outParam;
            _mockUserRepository
                .Setup(x => x.TryGetOne(It.IsAny<Guid>(), out outParam, It.IsAny<bool>()))
                .Returns(true);

            var userDto = new UserBuilder().WithId(Guid.NewGuid()).BuildDto();
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _userService.CreateAsync(userDto));
        }

        #endregion CreateAsync tests

        #region Email tests

        [Fact]
        public async Task UpdateEmailAddress_Should_InvokeUserRepositoryUpdate_Once()
        {
            string validEmailAddress = "joe.schreibvogel@tigerking.de";
            await _userService.UpdateEmailAddressAsync(_user.Id, validEmailAddress);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void UpdateEmailAddress_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateEmailAddressAsync(_user.Id, null));
            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Never);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmailAddress_Should_ThrowException_IfEmailIsInvalid()
        {
            string validEmailAddress = "INVALID_EMAIL";
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateEmailAddressAsync(_user.Id, validEmailAddress));
            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Never);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        #endregion Email tests

        #region Default ConsumerGroup tests

        [Fact]
        public async void GetDefaultConsumerGroup_Should_InvokeUserRepositoryGet_Once()
        {
            await _userService.GetDefaultConsumerGroupAsync(_user.Id);
            _mockUserRepository.Verify(x => x.GetDefaultConsumerGroupAsync(_user.Id), Times.Once);
        }

        [Fact]
        public async void UpdateDefaultConsumerGroup_Should_InvokeUserRepositoryUpdate_Once()
        {
            var cg = new ConsumerGroupBuilder().WithUri(new Uri("http://meh#234#433/213")).BuildDto();
            await _userService.UpdateDefaultConsumerGroupAsync(_user.Id, cg);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockConsumerGroupService.Verify(x => x.GetOne(It.IsAny<int>()), Times.Never);
            _mockConsumerGroupService.Verify(x => x.GetOne(It.IsAny<Uri>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void RemoveDefaultConsumerGroup_Should_InvokeUserRepositoryUpdate_Once()
        {
            await _userService.RemoveDefaultConsumerGroupAsync(_user.Id);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockConsumerGroupService.Verify(x => x.GetOne(It.IsAny<Uri>()), Times.Never);
            _mockUserRepository.Verify(x => x.UpdateReference(It.IsAny<User>(), It.IsAny<Expression<Func<User, EntityBase>>>()), Times.Once);
        }

        #endregion ConsumerGroup tests

        #region SearchFilterEditor tests

        [Fact]
        public void GetSearchFilterEditor_Should_InvokeUserRepositoryGet_Once()
        {
            _userService.GetSearchFilterEditorAsync(_user.Id);

            _mockUserRepository.Verify(x => x.GetSearchFilterEditorAsync(_user.Id), Times.Once);
        }

        [Fact]
        public void UpdateSearchFilterEditor_Should_InvokeUserRepositoryUpdate_Once()
        {
            var json = JObject.Parse(JsonString);
            _userService.UpdateSearchFilterEditorAsync(_user.Id, json);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void RemoveSearchFilterEditor_Should_InvokeUserRepositoryUpdate_Once()
        {
            _userService.RemoveSearchFilterEditorAsync(_user.Id);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateReference(It.IsAny<User>(), It.IsAny<Expression<Func<User, EntityBase>>>()), Times.Once);
        }

        #endregion SearchFilterEditor tests

        #region Search Filters Data Marketplace tests

        [Fact]
        public async void AddSearchFilterDataMarketplace_Should_InvokeUserRepositoryUpdate_Once()
        {
            // prepare mapping-mock required
            var sf = new SearchFilterDataMarketplaceBuilder().WithName("test").WithId(99);
            var sfEntity = sf.Build();

            var sfDto = sf.BuildDto();
            await _userService.AddSearchFilterDataMarketplaceAsync(_user.Id, sfDto);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void GetSearchFilterDataMarketplaceAsync_Should_InvokeUserRepositoryUpdate_Once()
        {
            await _userService.GetSearchFilterDataMarketplaceAsync(_user.Id, It.IsAny<int>());
            _mockUserRepository.Verify(x => x.GetSearchFilterDataMarketplaceAsync(_user.Id, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async void GetSearchFiltersDataMarketplaceAsync_Should_InvokeUserRepositoryUpdate_Once()
        {
            await _userService.GetSearchFiltersDataMarketplaceAsync(_user.Id);
            _mockUserRepository.Verify(x => x.GetSearchFiltersDataMarketplaceAsync(_user.Id), Times.Once);
        }

        [Fact]
        public async void UpdateSearchFilterDataMarketplace_Should_InvokeUserRepositoryUpdate_Once()
        {
            var sf = new Collection<SearchFilterDataMarketplace>
            {
                new SearchFilterDataMarketplaceBuilder().WithName("mumpitz").WithFilterJson(JObject.Parse("{\"value\":\"test\"}")).Build()
            };
            await _userService.UpdateSearchFiltersDataMarketplaceAsync(_user.Id, sf);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void RemoveSearchFilterDataMarketplace_Should_InvokeSearchFilterDataMarketplaceServiceDelete_Once()
        {
            var userId = Guid.NewGuid();
            var sfId = 99;
            var sfDM = new List<SearchFilterDataMarketplace>() { new SearchFilterDataMarketplaceBuilder().WithName("test").WithId(sfId).Build() };
            var user = new UserBuilder().WithId(userId).WithEmailAddress("cletus.ewing@grandtheftauto5.com").WithSearchFilterDataMarketplace(sfDM).Build();

            _mockUserRepository.Setup(x => x.GetOneAsync(userId, It.IsAny<bool>())).ReturnsAsync(user);

            await _userService.RemoveSearchFilterDataMarketplaceAsync(user.Id, sfId);

            _mockSearchFilterDataMarketplaceService.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async void RemoveSearchFilterDataMarketplace_ThrowsException_IfSearchFilterNotExists()
        {
            int nonExistingId = 991337;
            _mockSearchFilterDataMarketplaceService.Setup(x => x.Delete(nonExistingId)).Throws(new EntityNotFoundException());
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.RemoveSearchFilterDataMarketplaceAsync(_user.Id, nonExistingId));
        }

        #endregion Search Filters Data Marketplace tests


        #region Default Search Filters Data Marketplace tests

        [Fact]
        public async void GetDefaultSearchFilterDataMarketplace_Should_InvokeUserRepositoryGet_Once()
        {
            var searchFilter = TestData.GenerateRandomSearchFilterDataMarketplace();
            searchFilter.Id = 1;
            var user = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithSearchFilterDataMarketplace(new Collection<SearchFilterDataMarketplace>() { searchFilter })
                .WithDefaultSearchFilterDataMarketplaceId(searchFilter.Id)
                .Build();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);

            var resultSearchFilter = await _userService.GetDefaultSearchFilterDataMarketplaceAsync(user.Id);

            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            Assert.NotNull(resultSearchFilter);
            Assert.Equal(searchFilter.FilterJson, resultSearchFilter.FilterJson);
            Assert.Equal(searchFilter.Name, resultSearchFilter.Name);
        }

        [Fact]
        public async void UpdateDefaultSearchFilterDataMarketplace_Should_InvokeUserRepositoryUpdate_Once()
        {
            var searchFilter = TestData.GenerateRandomSearchFilterDataMarketplace();
            searchFilter.Id = 2;
            var user = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithSearchFilterDataMarketplace(new Collection<SearchFilterDataMarketplace>() { searchFilter })
                .Build();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            var updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);

            await _userService.UpdateDefaultSearchFilterDataMarketplaceAsync(user.Id, searchFilter.Id);

            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            Assert.Equal(searchFilter.Id, updatedUser.DefaultSearchFilterDataMarketplace);
        }

        [Fact]
        public async void RemoveDefaultSearchFilterDataMarketplace_Should_InvokeUserRepositoryUpdate_Once()
        {
            var searchFilter = TestData.GenerateRandomSearchFilterDataMarketplace();
            searchFilter.Id = 1;
            var user = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithSearchFilterDataMarketplace(new Collection<SearchFilterDataMarketplace>() { searchFilter })
                .WithDefaultSearchFilterDataMarketplaceId(searchFilter.Id)
                .Build();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            var updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);

            await _userService.RemoveDefaultSearchFilterDataMarketplaceAsync(user.Id);

            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            Assert.Null(updatedUser.DefaultSearchFilterDataMarketplace);
        }

        #endregion ConsumerGroup tests


        #region Colid Entry Subscriptions tests

        [Fact]
        public async void AddColidEntrySubscriptionAsync_Should_InvokeUserRepositoryUpdate_Once()
        {
            var ceDto = new ColidEntrySubscriptionBuilder().WithColidEntry(new Uri("http://meh.com/specialmeh")).BuildDto();
            await _userService.AddColidEntrySubscriptionAsync(_user.Id, ceDto);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void AddColidEntrySubscriptionAsync_ThrowsException_IfUserIsAlreadySubscribed()
        {
            var ceDto = new ColidEntrySubscriptionBuilder().WithColidEntry(new Uri("http://meh.com/specialmeh")).BuildDto();
            await _userService.AddColidEntrySubscriptionAsync(_user.Id, ceDto);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _userService.AddColidEntrySubscriptionAsync(_user.Id, ceDto));

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Exactly(2));
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void GetColidEntrySubscriptionsAsync_Should_InvokeUserRepositoryUpdate_Once()
        {
            var result = await _userService.GetColidEntrySubscriptionsAsync(_user.Id);
            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            Assert.NotEmpty(result);
            Assert.Equal(_colidEntrySubscription.ColidPidUri.AbsoluteUri, result.First().ColidPidUri.AbsoluteUri);
        }

        [Fact]
        public async void UpdateColidEntrySubscriptionsAsync_Should_InvokeUserRepositoryUpdate_Once()
        {
            var ces = new Collection<ColidEntrySubscription>
            {
                new ColidEntrySubscriptionBuilder().WithColidEntry("http://yesss.com").Build()
            };

            await _userService.UpdateColidEntrySubscriptionsAsync(_user.Id, ces);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void RemoveColidEntrySubscription_Should_InvokeColidEntrySubscriptionService_Once()
        {
            var colidPidUri = "http://hello.com/how_are_you#123";
            var ceBuilder = new ColidEntrySubscriptionBuilder().WithColidEntry(colidPidUri);
            var ceList = new List<ColidEntrySubscription>() { ceBuilder.WithId(99).Build() };
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmailAddress("cletus.ewing@grandtheftauto5.com").WithColidEntrySubscriptions(ceList).Build();
            _mockUserRepository.Setup(x => x.GetOneAsync(userId, It.IsAny<bool>())).ReturnsAsync(user);

            await _userService.RemoveColidEntrySubscriptionAsync(userId, ceBuilder.BuildDto());

            _mockColidEntrySubscriptionService.Verify(x => x.GetOne(It.IsAny<Guid>(), It.IsAny<ColidEntrySubscriptionDto>()), Times.Once);
            _mockColidEntrySubscriptionService.Verify(x => x.Delete(It.IsAny<ColidEntrySubscription>()), Times.Once);
            _mockUserRepository.Verify(x => x.GetOneAsync(userId, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async void RemoveColidEntrySubscription_ThrowsException_IfSearchFilterNotExists()
        {
            var userId = Guid.NewGuid();
            var dto = new ColidEntrySubscriptionBuilder().WithColidEntry("http://non.existing.uri/whats#up").BuildDto();
            var colidEntrySubscription = _mockColidEntrySubscriptionService.Setup(x => x.GetOne(userId, dto)).Throws(new EntityNotFoundException());
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.RemoveColidEntrySubscriptionAsync(userId, dto));
        }

        #endregion Colid Entry Subscriptions tests

        #region MessageConfig tests

        [Fact]
        public async void GetMessageConfig_Should_InvokeUserRepositoryGet_Once()
        {
            await _userService.GetMessageConfigAsync(_user.Id);
            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async void GetMessageConfig_ThrowsException_IfNoUserMessageConfigExists()
        {
            var userWithoutMessageConfig = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithEmailAddress("some.redneck@grandtheftauto5.com")
                .Build();

            _mockUserRepository.Setup(x => x.GetOneAsync(userWithoutMessageConfig.Id, It.IsAny<bool>()))
                .ReturnsAsync(userWithoutMessageConfig);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetMessageConfigAsync(userWithoutMessageConfig.Id));
            _mockUserRepository.Verify(x => x.GetOneAsync(userWithoutMessageConfig.Id, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async void UpdateMessageConfig_Should_InvokeUserRepositoryUpdate_Once()
        {
            var messageConfig = new MessageConfigDto()
            {
                SendInterval = SendInterval.Daily,
                DeleteInterval = DeleteInterval.Monthly
            };
            await _userService.UpdateMessageConfigAsync(_user.Id, messageConfig);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void UpdateMessageConfig_Should_UpdateDateForExistingMessages()
        {
            var message = TestData.GenerateSampleMessage();
            var originalSendOn = message.SendOn;
            var originalDeleteOn = message.DeleteOn;

            message.Id = 1;
            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update

            var messageConfig = new MessageConfigDto()
            {
                SendInterval = SendInterval.Monthly,
                DeleteInterval = DeleteInterval.Quarterly
            };
            await _userService.UpdateMessageConfigAsync(user.Id, messageConfig);

            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            Assert.True(updatedUser.Messages.First().SendOn < updatedUser.Messages.First().DeleteOn);
            // Assert.True(updatedUser.Messages.First().SendOn > originalSendOn); depend on date.now..
            Assert.True(updatedUser.Messages.First().DeleteOn > originalDeleteOn);
        }

        [Fact]
        public async void UpdateMessageConfig_ThrowsException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateMessageConfigAsync(_user.Id, null));
        }

        [Fact]
        public async void UpdateMessageConfig_ThrowsException_IfMessageConfigIsInvalid()
        {
            var invalidMessageConfig = new MessageConfigBuilder()
                .WithSendInterval(SendInterval.Monthly)
                .WithDeleteInterval(DeleteInterval.Weekly)
                .BuildDto();
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateMessageConfigAsync(_user.Id, invalidMessageConfig));
        }

        [Fact]
        public async void UpdateMessageConfig_ThrowsException_IfMessageConfigUnchanged()
        {
            var invalidMessageConfig = new MessageConfigBuilder()
                .WithSendInterval(SendInterval.Weekly)
                .WithDeleteInterval(DeleteInterval.Monthly)
                .BuildDto();
            await Assert.ThrowsAsync<EntityNotChangedException>(() => _userService.UpdateMessageConfigAsync(_user.Id, invalidMessageConfig));
        }

        #endregion MessageConfig tests

        #region Messages tests

        [Fact]
        public async void AddMessageAsync_Should_InvokeUserRepository_Once()
        {
            var msgDto = TestData.GenerateSampleMessageDto();
            await _userService.AddMessageAsync(_user.Id, msgDto);

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void GetMessagesAsync_Should_InvokeUserRepository_Once()
        {
            var message = TestData.GenerateSampleMessage();
            var user = new UserBuilder()
                .WithId(Guid.NewGuid())
                .WithEmailAddress("annoying.orange@weebls-stuff.com")
                .WithMessages(new Collection<Message> { message })
                .Build();

            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);

            var messageDtoResponse = await _userService.GetMessagesAsync(user.Id);
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);

            var responseMessage = messageDtoResponse.First();
            Assert.NotEmpty(messageDtoResponse);
            Assert.Equal(message.Body, responseMessage.Body);
            Assert.Equal(message.Subject, responseMessage.Subject);
            Assert.Equal(message.SendOn, responseMessage.SendOn);
            Assert.Equal(message.DeleteOn, responseMessage.DeleteOn);
            Assert.Equal(message.ReadOn, responseMessage.ReadOn);
        }

        [Fact]
        public async void DeleteMessageAsync_Should_UpdateUserRepository_Once()
        {
            // ARRANGE
            var message = TestData.GenerateSampleMessage();

            message.Id = 1;
            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update


            // ACT
            await _userService.DeleteMessageAsync(user.Id, 1);

            // ASSERT
            Assert.Empty(updatedUser.Messages);
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async void DeleteMessageAsReadAsync_ThrowsException_IfMessageIdNotExist()
        {
            // ARRANGE
            var user = TestData.GenerateRandomUser();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update

            // ACT
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.MarkMessageAsReadAsync(user.Id, 1));
        }

        [Fact]
        public async void MarkMessageAsReadAsync_Should_UpdateUserRepository_Once()
        {
            // ARRANGE 
            var message = TestData.GenerateSampleMessage();
            var originalReadOn = message.ReadOn;
            var originalSendOn = message.SendOn;
            var originalDeleteOn = message.DeleteOn;

            message.Id = 1;
            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            await _userService.MarkMessageAsReadAsync(user.Id, 1);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

            var updatedMessage = updatedUser.Messages.FirstOrDefault();
            Assert.NotNull(updatedMessage.ReadOn);
            Assert.Null(updatedMessage.SendOn);

            Assert.NotEqual(originalReadOn, updatedMessage.ReadOn);
            Assert.NotEqual(originalSendOn, updatedMessage.SendOn);
            Assert.Equal(originalDeleteOn, updatedMessage.DeleteOn);
        }

        [Fact]
        public async void MarkMessageAsReadAsync_Should_DoNothing_IfMessageWasAlreadyRead()
        {
            // ARRANGE
            var message = TestData.GenerateSampleMessage();
            message.ReadOn = DateTime.UtcNow;
            var originalReadOn = message.ReadOn;
            message.Id = 1;
            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update

            // ACT
            var resultMessage = await _userService.MarkMessageAsReadAsync(user.Id, 1);

            // ASSERT
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);

            Assert.Equal(originalReadOn, resultMessage.ReadOn);
        }

        [Fact]
        public async void MarkMessageAsReadAsync_ThrowsException_IfMessageIdNotExist()
        {
            var user = TestData.GenerateRandomUser();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.MarkMessageAsReadAsync(_user.Id, 968413248));
        }

        [Fact]
        public async void MarkMessagesAsReadAsync_Should_UpdateUserRepository_Once()
        {
            // ARRANGE 
            var firstMessage = TestData.GenerateSampleMessage();
            var firstOriginalReadOn = firstMessage.ReadOn;
            var firstOriginalSendOn = firstMessage.SendOn;
            var firstOriginalDeleteOn = firstMessage.DeleteOn;
            firstMessage.Id = 1;

            var secMessage = TestData.GenerateSampleMessage();
            var secOriginalReadOn = secMessage.ReadOn;
            var secOriginalSendOn = secMessage.SendOn;
            var secOriginalDeleteOn = secMessage.DeleteOn;
            secMessage.Id = 2;

            var thirdMessage = TestData.GenerateSampleMessage();
            thirdMessage.Id = 3;

            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message> { firstMessage, secMessage };
            var messageIds = new List<int> { 1, 2, 3 };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Returns<User>(u => u);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            var messages = await _userService.MarkMessagesAsReadAsync(user.Id, messageIds);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

            Assert.Equal(2, messages.Count);
            Assert.Single(messages, m => 
                m.Id == firstMessage.Id && firstOriginalReadOn != m.ReadOn && firstOriginalSendOn != m.SendOn && firstOriginalDeleteOn == m.DeleteOn
                );
            Assert.Single(messages, m =>
                m.Id == secMessage.Id && secOriginalReadOn != m.ReadOn && secOriginalSendOn != m.SendOn && secOriginalDeleteOn == m.DeleteOn
            );
        }

        [Fact]
        public async void MarkMessagesAsReadAsync_Should_DoNothing_IfMessageWasAlreadyRead()
        {
            // ARRANGE
            var firstMessage = TestData.GenerateSampleMessage();
            firstMessage.ReadOn = DateTime.UtcNow;
            var firstOriginalReadOn = firstMessage.ReadOn;
            firstMessage.Id = 1;

            var secMessage = TestData.GenerateSampleMessage();
            var secOriginalReadOn = secMessage.ReadOn;
            var secOriginalSendOn = secMessage.SendOn;
            var secOriginalDeleteOn = secMessage.DeleteOn;
            secMessage.Id = 2;

            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message> { firstMessage, secMessage };
            var messageIds = new List<int> { 1, 2, 3 };
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Returns<User>(u => u);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            var messages = await _userService.MarkMessagesAsReadAsync(user.Id, messageIds);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

            Assert.Equal(2, messages.Count);
            Assert.Single(messages, m =>
                m.Id == firstMessage.Id && firstOriginalReadOn == m.ReadOn);
            Assert.Single(messages, m =>
                m.Id == secMessage.Id && secOriginalReadOn != m.ReadOn && secOriginalSendOn != m.SendOn && secOriginalDeleteOn == m.DeleteOn
            );
        }

        [Fact]
        public async void MarkMessagesAsReadAsync_Should_DoNothing_IfMessageNotFound()
        {
            // ARRANGE
            var firstMessage = TestData.GenerateSampleMessage();
            firstMessage.Id = 1;

            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message> { firstMessage };
            var messageIds = new List<int> { 1, 2, 3 };

            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Returns<User>(u => u);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            var messages = await _userService.MarkMessagesAsReadAsync(user.Id, messageIds);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

            Assert.Single(messages);
            Assert.Single(messages, m => m.Id == firstMessage.Id);
        }

        [Fact]
        public async void MarkMessageAsSentAsync_Should_UpdateUserRepository_Once()
        {
            // ARRANGE 
            var message = TestData.GenerateSampleMessage();
            message.Id = 1;
            message.ReadOn = null;

            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };

            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            await _userService.MarkMessageAsSentAsync(user.Id, 1);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

            var updatedMessage = updatedUser.Messages.FirstOrDefault();
            Assert.NotNull(updatedMessage);
            Assert.NotNull(updatedMessage.ReadOn);
            Assert.Null(updatedMessage.SendOn);
        }

        [Fact]
        public async void MarkMessageAsSentAsync_Should_DoNothing_IfMessageAlreadyRead()
        {
            // ARRANGE 
            var message = TestData.GenerateSampleMessage();
            var messageReadOn = DateTime.Now;
            message.ReadOn = messageReadOn;
            message.SendOn = null;
            message.Id = 1;

            var user = TestData.GenerateRandomUser();
            user.Messages = new List<Message>() { message };

            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);
            User updatedUser = new User();
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()))
                .Callback<User>(callbackUser => updatedUser = callbackUser);
            _mockUserRepository.Setup(x => x.Exists(user.Id)).Returns(true); // required to call update 

            // ACT 
            var updatedMessage = await _userService.MarkMessageAsSentAsync(user.Id, 1);

            // ASSERT 
            _mockUserRepository.Verify(x => x.GetOneAsync(user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);

            Assert.NotNull(updatedMessage?.ReadOn);
            Assert.Null(updatedMessage.SendOn);
            Assert.Equal(messageReadOn, updatedMessage.ReadOn);
        }

        [Fact]
        public async void MarkMessageAsSentAsync_ThrowsException_IfMessageIdNotExist()
        {
            var user = TestData.GenerateRandomUser();
            _mockUserRepository.Setup(x => x.GetOneAsync(user.Id, It.IsAny<bool>())).ReturnsAsync(user);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.MarkMessageAsSentAsync(_user.Id, 968413248));
        }

        #endregion Messages tests

        [Fact]
        public void UpdateStoredQueries_Should_InvokeUserRepositoryUpdate_Once()
        {
            _userService.UpdateStoredQueriesAsync(_user.Id, It.IsAny<ICollection<StoredQuery>>());

            _mockUserRepository.Verify(x => x.GetOneAsync(_user.Id, It.IsAny<bool>()), Times.Once);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        }
    }
}
