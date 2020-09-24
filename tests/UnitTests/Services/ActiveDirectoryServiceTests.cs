using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Graph.Implementation;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.Cache.Services;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class ActiveDirectoryServiceTests
    {
        private readonly IActiveDirectoryService _activeDirectoryService;
        private readonly Mock<IRemoteGraphService> _mockRemoteGraphService;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<ILogger<ActiveDirectoryService>> _mockLogger;

        private readonly Microsoft.Graph.User _user;
        private readonly Microsoft.Graph.Group _group;
        private readonly string _managerId;

        public ActiveDirectoryServiceTests()
        {
            _mockRemoteGraphService = new Mock<IRemoteGraphService>();
            _mockLogger = new Mock<ILogger<ActiveDirectoryService>>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
            var mapper = new Mapper(configuration);
            //var cache = new NoCacheService();
            _mockCache = new Mock<ICacheService>();

            _activeDirectoryService = new ActiveDirectoryService(_mockRemoteGraphService.Object,
                mapper, _mockCache.Object, _mockLogger.Object);

            // Init testdata
            var inMemoryGraphService = new InMemoryGraphService();
            var userId = Guid.NewGuid().ToString();
            var groupId = Guid.NewGuid().ToString();

            _user = inMemoryGraphService.GetUserAsync(userId).Result;
            _group = inMemoryGraphService.GetGroupAsync(groupId).Result;
            _managerId = Guid.NewGuid().ToString();

            var users = inMemoryGraphService.FindActiveUserAsync(string.Empty).Result;
            var groups = inMemoryGraphService.FindGroupAsync(string.Empty).Result;

            // Init mock behaviour
            _mockRemoteGraphService.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_user));

            _mockRemoteGraphService.Setup(x => x.FindActiveUserAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(users));

            _mockRemoteGraphService.Setup(x => x.GetManagerIdOfUserAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_managerId));

            _mockRemoteGraphService.Setup(x => x.GetGroupAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_group));

            _mockRemoteGraphService.Setup(x => x.FindGroupAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(groups));
        }

        #region GetUser

        [Theory]
        [InlineData("b9a5fd44-6c9f-43a8-8bc6-0844fbe3e5a6")]
        [InlineData("peter.johnson@bayer.com")]
        public async Task GetUser_Should_InvokeRemoteGraphServiceGetUser_Once(string id)
        {
            await _activeDirectoryService.GetUserAsync(id);
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetUserAsync(null));
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfArgumentIsEmptyString()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetUserAsync(string.Empty));
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfArgumentIsWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetUserAsync(" "));
        }

        [Fact]
        public async Task GetUserById_Should_ThrowException_IfEntityNotFound()
        {
            _user.AccountEnabled = false;

            _mockRemoteGraphService.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_user));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _activeDirectoryService.GetUserAsync("peter.johnson@bayer.com"));
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfArgumentIsInvalid()
        {
            await Assert.ThrowsAsync<FormatException>(() => _activeDirectoryService.GetUserAsync("invalid-id"));
        }

        #endregion GetUser

        #region GetManagerByUserId

        [Theory]
        [InlineData("b9a5fd44-6c9f-43a8-8bc6-0844fbe3e5a6")]
        [InlineData("peter.johnson@bayer.com")]
        public async Task GetManagerByUserId_Should_InvokeRemoteGraphServiceGetManagerIdOfUser_Once(string id)
        {
            await _activeDirectoryService.GetManagerByUserIdAsync(id);
            _mockRemoteGraphService.Verify(x => x.GetManagerIdOfUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(_managerId), Times.Once);
        }

        [Fact]
        public async Task GetManagerByUserId_Should_ThrowException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetManagerByUserIdAsync(null));
        }

        [Fact]
        public async Task GetManagerByUserId_Should_ThrowException_IfArgumentIsEmptyString()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetManagerByUserIdAsync(string.Empty));
        }

        [Fact]
        public async Task GetManagerByUserId_Should_ThrowException_IfArgumentIsWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetManagerByUserIdAsync(" "));
        }

        [Fact]
        public async Task GetManagerByUserIdr_Should_ThrowException_IfArgumentIsInvalid()
        {
            await Assert.ThrowsAsync<FormatException>(() => _activeDirectoryService.GetManagerByUserIdAsync("invalid-id"));
        }

        #endregion GetManagerByUserId

        #region GetUsers

        [Fact]
        public async Task GetUsers_Should_InvokeRemoteGraphServiceSearchUser_Once()
        {
            var query = "Peter";
            await _activeDirectoryService.FindUsersAsync(query);
            _mockRemoteGraphService.Verify(x => x.FindActiveUserAsync(query), Times.Once);
        }

        [Fact]
        public async Task GetUsers_Should_ThrowException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindUsersAsync(null));
        }

        [Fact]
        public async Task GetUsers_Should_ThrowException_IfArgumentIsEmptyString()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindUsersAsync(string.Empty));
        }

        [Fact]
        public async Task GetUsers_Should_ThrowException_IfArgumentIsWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindUsersAsync(" "));
        }

        #endregion GetUsers

        #region GetGroup

        [Fact]
        public async Task GetGroup_Should_InvokeRemoteGraphServiceGetGroup_Once()
        {
            var id = "demo.group@bayer.com";
            await _activeDirectoryService.GetGroupAsync(id);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetGroupAsync(null));
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfArgumentIsEmptyString()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetGroupAsync(string.Empty));
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfArgumentIsWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.GetGroupAsync(" "));
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfEntityNotFound()
        {
            _group.MailEnabled = false;

            _mockRemoteGraphService.Setup(x => x.GetGroupAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_group));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _activeDirectoryService.GetGroupAsync("demo.group@bayer.com"));
        }

        [Theory]
        [InlineData("invalidemail.com")]
        [InlineData("b9a5fd44-6c9f-43a8-8bc6-0844fbe3e5a6")]
        public async Task GetGroup_Should_ThrowException_IfArgumentIsInvalid(string id)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _activeDirectoryService.GetGroupAsync(id));
        }

        #endregion GetGroup

        #region GetGroups

        [Fact]
        public async Task GetGroups_Should_InvokeRemoteGraphServiceGetGroups_Once()
        {
            var query = "bayer";
            await _activeDirectoryService.FindGroupsAsync(query);
            _mockRemoteGraphService.Verify(x => x.FindGroupAsync(query), Times.Once);
        }

        [Fact]
        public async Task GetGroups_Should_ThrowException_IfArgumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindGroupsAsync(null));
        }

        [Fact]
        public async Task GetGroups_Should_ThrowException_IfArgumentIsEmptyString()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindGroupsAsync(string.Empty));
        }

        [Fact]
        public async Task GetGroups_Should_ThrowException_IfArgumentIsWhitespace()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _activeDirectoryService.FindGroupsAsync(" "));
        }

        #endregion GetGroups

        #region GetUsersAndGroups

        [Fact]
        public void GetUsersAndGroups_Should_InvokeRemoteGraphService_Once()
        {
            var query = "bayer";
            _activeDirectoryService.FindUsersAndGroupsAsync(query);
            _mockRemoteGraphService.Verify(x => x.FindActiveUserAsync(query), Times.Once);
            _mockRemoteGraphService.Verify(x => x.FindGroupAsync(query), Times.Once);
        }

        [Fact]
        public void GetUsersAndGroups_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.FindUsersAndGroupsAsync(null));
        }

        [Fact]
        public void GetUsersAndGroups_Should_ThrowException_IfArgumentIsEmptyString()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.FindUsersAndGroupsAsync(string.Empty));
        }

        [Fact]
        public void GetUsersAndGroups_Should_ThrowException_IfArgumentIsWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.FindUsersAndGroupsAsync(" "));
        }

        [Fact]
        public void GetUsersAndGroups_Should_InvokeRemoteGraphService_SearchUserThrowsException_Once()
        {
            // Arrange
            var query = "bayer";

            _mockRemoteGraphService.Setup(x => x.FindActiveUserAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act
            var result = _activeDirectoryService.FindUsersAndGroupsAsync(query);

            // Assert
            _mockRemoteGraphService.Verify(x => x.FindActiveUserAsync(query), Times.Once);
            _mockRemoteGraphService.Verify(x => x.FindGroupAsync(query), Times.Once);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUsersAndGroups_Should_InvokeRemoteGraphService_SearchGroupThrowsException_Once()
        {
            // Arrange
            var query = "bayer";

            _mockRemoteGraphService.Setup(x => x.FindGroupAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act
            var result = _activeDirectoryService.FindUsersAndGroupsAsync(query);

            // Assert
            _mockRemoteGraphService.Verify(x => x.FindActiveUserAsync(query), Times.Once);
            _mockRemoteGraphService.Verify(x => x.FindGroupAsync(query), Times.Once);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUsersAndGroups_Should_ThrowException_IfEntityNotFound()
        {
            // Arrange
            var query = "bayer";

            _mockRemoteGraphService.Setup(x => x.FindActiveUserAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));
            _mockRemoteGraphService.Setup(x => x.FindGroupAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act & Assert
            Assert.Throws<EntityNotFoundException>(() => _activeDirectoryService.FindUsersAndGroupsAsync(query));
            _mockRemoteGraphService.Verify(x => x.FindActiveUserAsync(query), Times.Once);
            _mockRemoteGraphService.Verify(x => x.FindGroupAsync(query), Times.Once);
        }

        #endregion GetUsersAndGroups

        #region GetUserOrGroup

        [Fact]
        public void GetUserOrGroup_Should_InvokeRemoteGraphService_Once()
        {
            var id = "peter.johnson@bayer.com";

            _activeDirectoryService.GetUserOrGroupAsync(id);
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Once);
        }

        [Fact]
        public void GetUserOrGroup_Should_InvokeRemoteGraphService_GetUser_Once()
        {
            var id = "b9a5fd44-6c9f-43a8-8bc6-0844fbe3e5a6";

            _activeDirectoryService.GetUserOrGroupAsync(id);
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Never);
        }

        [Fact]
        public void GetUserOrGroup_Should_ThrowException_IfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.GetUserOrGroupAsync(null));
        }

        [Fact]
        public void GetUserOrGroup_Should_ThrowException_IfArgumentIsEmptyString()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.GetUserOrGroupAsync(string.Empty));
        }

        [Fact]
        public void GetUserOrGroup_Should_ThrowException_IfArgumentIsWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() => _activeDirectoryService.GetUserOrGroupAsync(" "));
        }

        [Fact]
        public void GetUserOrGroup_Should_ThrowException_IfArgumentIsInvalid()
        {
            Assert.Throws<FormatException>(() => _activeDirectoryService.GetUserOrGroupAsync("invalid-email"));
        }

        [Fact]
        public void GetUserOrGroup_Should_InvokeRemoteGraphService_GetUserThrowsException_Once()
        {
            // Arrange
            var id = "peter.johnson@bayer.com";

            _mockRemoteGraphService.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act
            var result = _activeDirectoryService.GetUserOrGroupAsync(id);

            // Assert
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Once);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserOrGroup_Should_InvokeRemoteGraphService_GetGroupThrowsException_Once()
        {
            // Arrange
            var id = "peter.johnson@bayer.com";

            _mockRemoteGraphService.Setup(x => x.GetGroupAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act
            var result = _activeDirectoryService.GetUserOrGroupAsync(id);

            // Assert
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Once);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserOrGroup_Should_ThrowException_IfEntityNotFound()
        {
            // Arrange
            var id = "peter.johnson@bayer.com";

            _mockRemoteGraphService.Setup(x => x.GetUserAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));
            _mockRemoteGraphService.Setup(x => x.GetGroupAsync(It.IsAny<string>()))
                .Throws(new EntityNotFoundException("Not found"));

            // Act & Assert
            Assert.Throws<EntityNotFoundException>(() => _activeDirectoryService.GetUserOrGroupAsync(id));
            _mockRemoteGraphService.Verify(x => x.GetUserAsync(id), Times.Once);
            _mockRemoteGraphService.Verify(x => x.GetGroupAsync(id), Times.Once);
        }

        #endregion GetUserOrGroup

        #region CheckUsersValidityAsync

        [Fact]
        public async Task CheckUsersValidityAsync_Should_InvokeRemoteGraphService_Once_WhenAllEmailsValid()
        {
            // ARRANGE
            string _;
            _mockCache.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            var validEmails = new HashSet<string>() { "valid.email.one@bayer.com" };
            var adUserCheckResult = new List<AdUserDto>() { new AdUserDto(Guid.NewGuid().ToString(), validEmails.First(), true) };
            _mockRemoteGraphService.Setup(x => x.CheckUsersValidityAsync(validEmails)).ReturnsAsync(adUserCheckResult);

            // ACT
            var resultList = await _activeDirectoryService.CheckUsersValidityAsync(validEmails);
            
            // ASSERT
            _mockCache.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
            _mockRemoteGraphService.Verify(x => x.CheckUsersValidityAsync(It.IsAny<HashSet<string>>()), Times.Once);

            Assert.NotNull(resultList);
            Assert.Single(resultList);
            Assert.NotNull(resultList.FirstOrDefault(x => x.Mail == validEmails.First()));
        }

        [Fact]
        public async Task CheckUsersValidityAsync_Should_InvokeRemoteGraphService_Never_WhenAllEmailsInvalid()
        {
            // ARRANGE
            var invalidUserList = new HashSet<string> { "invalid.email.one@bayer.com", "invalid.email.two@bayer.com" };
            _mockCache.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            // ACT
            var resultList = await _activeDirectoryService.CheckUsersValidityAsync(invalidUserList);

            // ASSERT
            _mockCache.Verify(x => x.Exists(It.IsAny<string>()), Times.Exactly(2));
            _mockRemoteGraphService.Verify(x => x.CheckUsersValidityAsync(It.IsAny<HashSet<string>>()), Times.Never);
            
            Assert.NotNull(resultList);
            Assert.Equal(2, resultList.Count());
            Assert.NotNull(resultList.FirstOrDefault(x => x.Mail == invalidUserList.First()));
        }

        [Fact]
        public async Task CheckUsersValidityAsync_Should_InvokeRemoteGraphService_Once_WhenAllEmailsPartialInvalid()
        {
            // ARRANGE
            const string invalidEmailOne = "invalid.email.one@bayer.com";
            const string invalidEmailTwo = "invalid.email.two@bayer.com";
            const string validEmail = "valid.email@bayer.com";

            _mockCache.Setup(x => x.Exists(invalidEmailOne)).Returns(true);
            _mockCache.Setup(x => x.Exists(invalidEmailTwo)).Returns(true);
            _mockCache.Setup(x => x.Exists(validEmail)).Returns(false);

            var adUserCheckResult = new List<AdUserDto>() { new AdUserDto(Guid.NewGuid().ToString(), validEmail, true) };
            _mockRemoteGraphService
                .Setup(x => x.CheckUsersValidityAsync(new HashSet<string>() { validEmail }))
                .ReturnsAsync(adUserCheckResult);

            var emailsToCheck = new HashSet<string> { validEmail, invalidEmailOne, invalidEmailTwo };

            // ACT
            var resultList = await _activeDirectoryService.CheckUsersValidityAsync(emailsToCheck);

            // ASSERT
            _mockCache.Verify(x => x.Exists(It.IsAny<string>()), Times.Exactly(3));
            _mockRemoteGraphService.Verify(x => x.CheckUsersValidityAsync(It.IsAny<HashSet<string>>()), Times.Once);

            Assert.NotNull(resultList);
            Assert.Equal(3, resultList.Count());
            Assert.NotNull(resultList.FirstOrDefault(x => x.Mail == invalidEmailOne));
            Assert.NotNull(resultList.FirstOrDefault(x => x.Mail == invalidEmailTwo));
            Assert.NotNull(resultList.FirstOrDefault(x => x.Mail == validEmail));
        }

        [Theory]
        [InlineData("p eter.johnson.ext@bayer.com")]
        [InlineData("peter.johnson.ext@bayer.com ")]
        [InlineData("123jldkalsdjlas")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void CheckUsersValidityAsync_Should_ThrowException_IfEmailIsInvalid(string arg)
        {
            var emailSet = new HashSet<string>() { arg };
            Assert.ThrowsAsync<ArgumentException>(() => _activeDirectoryService.CheckUsersValidityAsync(emailSet));
        }
        
        #endregion
    }
}
