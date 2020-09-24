using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Services.Graph.Implementation;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Core.Requests;
using Moq;
using Xunit;

namespace COLID.AppDataService.Tests.Unit.Services
{
    public class RemoteGraphServiceTests
    {
        private readonly IRemoteGraphService _remoteGraphService;
        private readonly Mock<IGraphServiceClient> _graphServiceClient;
        private readonly Mock<ILogger<RemoteMicrosoftGraphService>> _logger;

        private readonly User _user;
        private readonly IList<User> _users;
        private readonly Group _group;
        private readonly IList<Group> _groups;
        private readonly string _managerId;

        private readonly Mock<IUserRequest> _mockUserRequest;
        private readonly Mock<IUserRequestBuilder> _mockUserRequestBuilder;

        private readonly Mock<IGraphServiceUsersCollectionRequest> _mockUsersCollectionRequest;
        private readonly Mock<IGraphServiceUsersCollectionRequestBuilder> _mockUsersCollectionRequestBuilder;

        private readonly Mock<IGraphServiceGroupsCollectionRequest> _mockGroupsCollectionRequest;
        private readonly Mock<IGraphServiceGroupsCollectionRequestBuilder> _mockGroupsCollectionRequestBuilder;

        private readonly Mock<IDirectoryObjectWithReferenceRequest> _mockReferenceRequest;
        private readonly Mock<IDirectoryObjectWithReferenceRequestBuilder> _mockReferenceRequestBuilder;

        private readonly Mock<IBaseClient> _mockBaseClient;
        private readonly Mock<IBatchRequest> _mockBatchRequest;
        private readonly Mock<IBatchRequestBuilder> _mockBatchRequestBuilder;

        public RemoteGraphServiceTests()
        {
            _logger = new Mock<ILogger<RemoteMicrosoftGraphService>>();
            _graphServiceClient = new Mock<IGraphServiceClient>();
            _remoteGraphService = new RemoteMicrosoftGraphService(_graphServiceClient.Object, _logger.Object);
            var inMemoryGraphService = new InMemoryGraphService();

            _user = inMemoryGraphService.GetUserAsync(Guid.NewGuid().ToString()).Result;
            _users = inMemoryGraphService.FindActiveUserAsync(string.Empty).Result;
            _group = inMemoryGraphService.GetGroupAsync(Guid.NewGuid().ToString()).Result;
            _groups = inMemoryGraphService.FindGroupAsync(string.Empty).Result;

            _managerId = Guid.NewGuid().ToString();

            _mockBatchRequest = new Mock<IBatchRequest>();
            var httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
            httpResponseMessage.ReasonPhrase = "Not Found";
            var batchResponseContent = new BatchResponseContent(httpResponseMessage);
            _mockBatchRequest.Setup(x => x.PostAsync(It.IsAny<BatchRequestContent>()))
                .ReturnsAsync(batchResponseContent);

            _mockBatchRequestBuilder = new Mock<IBatchRequestBuilder>();
            _mockBatchRequestBuilder.Setup(x => x.Request()).Returns(_mockBatchRequest.Object);

            _mockBaseClient = new Mock<IBaseClient>();
            _mockBaseClient.Setup(x => x.Batch).Returns(_mockBatchRequestBuilder.Object);

            _mockUserRequest = new Mock<IUserRequest>();
            _mockUserRequest.Setup(x => x.Select(It.IsAny<Expression<Func<User, object>>>())).Returns(_mockUserRequest.Object);
            _mockUserRequest.Setup(x => x.GetAsync()).ReturnsAsync(_user);
            _mockUserRequest.Setup(x => x.GetHttpRequestMessage()).Returns(new HttpRequestMessage(new HttpMethod("GET"), new Uri("http://localhost/")));

            _mockReferenceRequest = new Mock<IDirectoryObjectWithReferenceRequest>();
            _mockReferenceRequest.Setup(x => x.Select(It.IsAny<Expression<Func<DirectoryObject, object>>>())).Returns(_mockReferenceRequest.Object);
            _mockReferenceRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(new DirectoryObject() { Id = _managerId }));

            _mockReferenceRequestBuilder = new Mock<IDirectoryObjectWithReferenceRequestBuilder>();
            _mockReferenceRequestBuilder.Setup(x => x.Request()).Returns(_mockReferenceRequest.Object);

            _mockUserRequestBuilder = new Mock<IUserRequestBuilder>();
            _mockUserRequestBuilder.Setup(x => x.Request()).Returns(_mockUserRequest.Object);
            _mockUserRequestBuilder.Setup(x => x.Manager).Returns(_mockReferenceRequestBuilder.Object);

            _mockUsersCollectionRequest = new Mock<IGraphServiceUsersCollectionRequest>();
            _mockUsersCollectionRequest.Setup(x => x.Select(It.IsAny<Expression<Func<User, object>>>())).Returns(_mockUsersCollectionRequest.Object);
            _mockUsersCollectionRequest.Setup(x => x.Filter(It.IsAny<string>())).Returns(_mockUsersCollectionRequest.Object);
            _mockUsersCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(GetUsersCollectionPage()));

            _mockUsersCollectionRequestBuilder = new Mock<IGraphServiceUsersCollectionRequestBuilder>();
            _mockUsersCollectionRequestBuilder.Setup(x => x.Request()).Returns(_mockUsersCollectionRequest.Object);

            _mockGroupsCollectionRequest = new Mock<IGraphServiceGroupsCollectionRequest>();
            _mockGroupsCollectionRequest.Setup(x => x.Select(It.IsAny<Expression<Func<Group, object>>>())).Returns(_mockGroupsCollectionRequest.Object);
            _mockGroupsCollectionRequest.Setup(x => x.Filter(It.IsAny<string>())).Returns(_mockGroupsCollectionRequest.Object);
            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(GetGroupsCollectionPage()));

            _mockGroupsCollectionRequestBuilder = new Mock<IGraphServiceGroupsCollectionRequestBuilder>();
            _mockGroupsCollectionRequestBuilder.Setup(x => x.Request()).Returns(_mockGroupsCollectionRequest.Object);

            _graphServiceClient.Setup(x => x.Users)
                .Returns(_mockUsersCollectionRequestBuilder.Object);

            _graphServiceClient.Setup(x => x.Users[It.IsAny<string>()])
                .Returns(_mockUserRequestBuilder.Object);

            _graphServiceClient.Setup(x => x.Groups)
                .Returns(_mockGroupsCollectionRequestBuilder.Object);

            _graphServiceClient.Setup(x => x.Batch)
                .Returns(_mockBatchRequestBuilder.Object);
        }

        private IGraphServiceGroupsCollectionPage GetGroupsCollectionPage()
        {
            IGraphServiceGroupsCollectionPage groups = new GraphServiceGroupsCollectionPage();
            foreach (var group in _groups)
            {
                groups.Add(group);
            }
            return groups;
        }

        private IGraphServiceUsersCollectionPage GetUsersCollectionPage()
        {
            IGraphServiceUsersCollectionPage users = new GraphServiceUsersCollectionPage();
            foreach (var user in _users)
            {
                users.Add(user);
            }
            return users;
        }

        #region GetUser

        [Fact]
        public async Task GetUser_Should_InvokeRemoteGraphServiceGetAsync_Once()
        {
            await _remoteGraphService.GetUserAsync(_user.Id);
            _graphServiceClient.Verify(x => x.Users[_user.Id], Times.Once);
            _mockUserRequestBuilder.Verify(x => x.Request(), Times.Once);
            _mockUserRequest.Verify(x => x.GetAsync(), Times.Once);
        }

        [Theory]
        [InlineData(MicrosoftConstants.Exception.Codes.RequestResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ErrorItemNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ItemNotFound)]
        public async Task GetUser_Should_ThrowException_IfNotFound(string errorCode)
        {
            var graphError = new Error() { Code = errorCode };
            var serviceException = new ServiceException(graphError);

            _mockUserRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.GetUserAsync(_user.Id));
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfBadRequest()
        {
            var graphError = new Error() { Code = MicrosoftConstants.Exception.Codes.BadRequest };
            var serviceException = new ServiceException(graphError);

            _mockUserRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<QueryException>(() => _remoteGraphService.GetUserAsync(_user.Id));
        }

        [Fact]
        public async Task GetUser_Should_ThrowException_IfUnspecifiedError()
        {
            var graphError = new Error() { Code = "UnkownError" };
            var serviceException = new ServiceException(graphError);

            _mockUserRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<System.Exception>(() => _remoteGraphService.GetUserAsync(_user.Id));
        }

        #endregion GetUser

        #region GetManagerIdOfUser

        [Fact]
        public async Task GetManagerIdOfUser_Should_InvokeRemoteGraphServiceGetUser_Once()
        {
            await _remoteGraphService.GetManagerIdOfUserAsync(_user.Id);
            _graphServiceClient.Verify(x => x.Users[_user.Id], Times.Once);
            _mockUserRequestBuilder.Verify(x => x.Manager, Times.Once);
            _mockReferenceRequestBuilder.Verify(x => x.Request(), Times.Once);
            _mockReferenceRequest.Verify(x => x.Select(i => i.Id), Times.Once);
            _mockReferenceRequest.Verify(x => x.GetAsync(), Times.Once);
        }

        [Theory]
        [InlineData(MicrosoftConstants.Exception.Codes.RequestResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ErrorItemNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ItemNotFound)]
        public async Task GetManagerIdOfUser_Should_ThrowException_IfNotFound(string errorCode)
        {
            var graphError = new Error() { Code = errorCode };
            var serviceException = new ServiceException(graphError);

            _mockReferenceRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.GetManagerIdOfUserAsync(_user.Id));
        }

        [Fact]
        public async Task GetManagerIdOfUser_Should_ThrowException_IfBadRequest()
        {
            var graphError = new Error() { Code = MicrosoftConstants.Exception.Codes.BadRequest };
            var serviceException = new ServiceException(graphError);

            _mockReferenceRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<QueryException>(() => _remoteGraphService.GetManagerIdOfUserAsync(_user.Id));
        }

        [Fact]
        public async Task GetManagerIdOfUser_Should_ThrowException_IfUnspecifiedError()
        {
            var graphError = new Error() { Code = "UnkownError" };
            var serviceException = new ServiceException(graphError);

            _mockReferenceRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<System.Exception>(() => _remoteGraphService.GetManagerIdOfUserAsync(_user.Id));
        }

        #endregion GetManagerIdOfUser

        #region FindUserAsync

        [Fact]
        public async Task FindUserAsync_Should_InvokeRemoteGraphServiceGetAsync_Once()
        {
            await _remoteGraphService.FindActiveUserAsync(_user.Id);
            _graphServiceClient.Verify(x => x.Users, Times.Once);
            _mockUsersCollectionRequestBuilder.Verify(x => x.Request(), Times.Once);
            _mockUsersCollectionRequest.Verify(x => x.GetAsync(), Times.Once);
        }

        [Fact]
        public async Task FindUserAsync_Should_ThrowException_IfResultIsNull()
        {
            IGraphServiceUsersCollectionPage collectionPage = null;
            _mockUsersCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(collectionPage));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.FindActiveUserAsync(string.Empty));
        }

        [Fact]
        public async Task FindUserAsync_Should_ThrowException_IfResultIsEmpty()
        {
            IGraphServiceUsersCollectionPage collectionPage = new GraphServiceUsersCollectionPage();
            _mockUsersCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(collectionPage));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.FindActiveUserAsync(string.Empty));
        }

        [Fact]
        public async Task FindUserAsync_Should_ThrowException_IfBadRequest()
        {
            var graphError = new Error() { Code = MicrosoftConstants.Exception.Codes.BadRequest };
            var serviceException = new ServiceException(graphError);

            _mockUsersCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<QueryException>(() => _remoteGraphService.FindActiveUserAsync(string.Empty));
        }

        [Fact]
        public async Task FindUserAsync_Should_ThrowException_IfUnspecifiedError()
        {
            var graphError = new Error() { Code = "UnkownError" };
            var serviceException = new ServiceException(graphError);

            _mockUsersCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<System.Exception>(() => _remoteGraphService.FindActiveUserAsync(string.Empty));
        }

        #endregion FindUserAsync

        #region GetGroup

        [Fact]
        public async Task GetGroup_Should_InvokeRemoteGraphServiceGetAsync_Once()
        {
            await _remoteGraphService.GetGroupAsync(_group.Id);
            _mockGroupsCollectionRequestBuilder.Verify(x => x.Request(), Times.Once);
            _mockGroupsCollectionRequest.Verify(x => x.GetAsync(), Times.Once);
        }

        [Theory]
        [InlineData(MicrosoftConstants.Exception.Codes.RequestResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ResourceNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ErrorItemNotFound)]
        [InlineData(MicrosoftConstants.Exception.Codes.ItemNotFound)]
        public async Task GetGroup_Should_ThrowException_IfNotFound(string errorCode)
        {
            var graphError = new Error() { Code = errorCode };
            var serviceException = new ServiceException(graphError);

            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.GetGroupAsync(_group.Id));
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfBadRequest()
        {
            var graphError = new Error() { Code = MicrosoftConstants.Exception.Codes.BadRequest };
            var serviceException = new ServiceException(graphError);

            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<QueryException>(() => _remoteGraphService.GetGroupAsync(_group.Id));
        }

        [Fact]
        public async Task GetGroup_Should_ThrowException_IfUnspecifiedError()
        {
            var graphError = new Error() { Code = "UnkownError" };
            var serviceException = new ServiceException(graphError);

            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<System.Exception>(() => _remoteGraphService.GetGroupAsync(_group.Id));
        }

        #endregion GetGroup

        #region FindGroup

        [Fact]
        public async Task FindGroup_Should_InvokeRemoteGraphServiceGetAsync_Once()
        {
            await _remoteGraphService.FindGroupAsync(string.Empty);
            _mockGroupsCollectionRequestBuilder.Verify(x => x.Request(), Times.Once);
            _mockGroupsCollectionRequest.Verify(x => x.GetAsync(), Times.Once);
        }

        [Fact]
        public async Task FindGroup_Should_ThrowException_IfResultIsNull()
        {
            IGraphServiceGroupsCollectionPage collectionPage = null;
            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(collectionPage));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.FindGroupAsync(string.Empty));
        }

        [Fact]
        public async Task FindGroup_Should_ThrowException_IfResultIsEmpty()
        {
            IGraphServiceGroupsCollectionPage collectionPage = new GraphServiceGroupsCollectionPage();
            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Returns(Task.FromResult(collectionPage));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _remoteGraphService.FindGroupAsync(string.Empty));
        }

        [Fact]
        public async Task FindGroup_Should_ThrowException_IfBadRequest()
        {
            var graphError = new Error() { Code = MicrosoftConstants.Exception.Codes.BadRequest };
            var serviceException = new ServiceException(graphError);

            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<QueryException>(() => _remoteGraphService.FindGroupAsync(string.Empty));
        }

        [Fact]
        public async Task FindGroup_Should_ThrowException_IfUnspecifiedError()
        {
            var graphError = new Error() { Code = "UnkownError" };
            var serviceException = new ServiceException(graphError);

            _mockGroupsCollectionRequest.Setup(x => x.GetAsync()).Throws(serviceException);

            await Assert.ThrowsAsync<System.Exception>(() => _remoteGraphService.FindGroupAsync(string.Empty));
        }

        #endregion FindGroup

        #region CheckUsersValidityAsync

        [Fact]
        public async Task CheckUsersValidityAsync_Should_Run()
        {
            var emailList = new HashSet<string>() { "bli@bla.com" };
            var resultList = await _remoteGraphService.CheckUsersValidityAsync(emailList);
            Assert.NotNull(resultList);
        }

        #endregion 
    }
}
