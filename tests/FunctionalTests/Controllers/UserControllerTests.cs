using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class UserControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/users";

        private readonly IMapper _mapper;
        private readonly TestDataContextSeeder _seeder;
        private readonly Random _rnd;
        private readonly ApiTestHelper _api;
        private readonly ITestOutputHelper _output;

        public UserControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _mapper = (IMapper)factory.Services.GetService(typeof(IMapper));
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;
            _rnd = new Random();
        }

        #region Users | /api/users/{userId}

        [Fact]
        public async Task Users_Get_All_Returns_Ok()
        {
            _seeder.ResetUsers(); // Reset to initial, so delete and create doesn't change the value
            var response = await Client.GetAsync(PATH);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<List<User>>(stringResponse);
            Assert.Equal(5, users.Count());
        }

        [Fact]
        public async Task Users_Get_Single_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());

            var response = await Client.GetAsync($"{PATH}/{user.Id}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.Equal(user.Id, responseUser.Id);
            Assert.Equal(user.EmailAddress, responseUser.EmailAddress);
        }

        [Fact]
        public async Task Users_Get_Single_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Users_Get_Single_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/bliblablubb");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Users_Post_Returns_Ok()
        {
            var user = TestData.GenerateRandomUser();

            var response = await Client.PostAsync(PATH, BuildJsonHttpContent(user));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.Equal(user.Id, responseUser.Id);
            Assert.Equal(user.EmailAddress, responseUser.EmailAddress);
        }

        [Fact]
        public async Task Users_Post_Returns_BadRequest_IfContentIsNull()
        {
            var response = await Client.PostAsync($"{PATH}", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Users_Post_Returns_MethodNotAllowed_IfContentIsEmpty()
        {
            var response = await Client.PutAsync($"{PATH}", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task Users_Post_Returns_MethodNotAllowed_IfContentIsPlainText()
        {
            var response = await Client.PutAsync($"{PATH}", BuildPlainTextHttpContent("123"));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task Users_Delete_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Users_Delete_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.DeleteAsync($"{PATH}/{Guid.NewGuid()}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Users_Delete_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.DeleteAsync($"{PATH}/somefancyguid");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Users | /api/users/{userId}

        #region Email-Address | /api/users/{userId}/emailAddress

        [Fact]
        public async Task UpdateEmailAddress_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var originalEmail = user.EmailAddress;
            var updatedmailAddress = "jeff.lowe@tigerking.de";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/emailAddress", BuildPlainTextHttpContent(updatedmailAddress));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEqual(originalEmail, responseUser.EmailAddress);
            Assert.Equal(updatedmailAddress, responseUser.EmailAddress);
        }

        [Fact]
        public async Task UpdateEmailAddress_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/emailAddress", BuildPlainTextHttpContent("valid@email.address"));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEmailAddress_Put_Returns_BadRequest_IfEmailPatternMismatches()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var updatedmailAddress = "thisisnotanemailaddress";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/emailAddress", BuildPlainTextHttpContent(updatedmailAddress));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEmailAddress_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/emailAddress", BuildPlainTextHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEmailAddress_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/emailAddress", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Email-Address | /api/users/{userId}/emailAddress

        #region Default Consumer Group | /api/users/{userId}/defaultConsumerGroup

        [Fact]
        public async Task UpdateDefaultConsumerGroup_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var cg = _seeder.Add(TestData.GenerateRandomConsumerGroup());
            var cgDto = new ConsumerGroupBuilder().WithUri(cg.Uri).BuildDto();
            var originalCg = user.DefaultConsumerGroup;

            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultConsumerGroup", BuildJsonHttpContent(cgDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEqual(originalCg, responseUser.DefaultConsumerGroup);
            Assert.Equal(cgDto.Uri.AbsoluteUri, responseUser.DefaultConsumerGroup.Uri.AbsoluteUri);
        }

        [Fact]
        public async Task UpdateDefaultConsumerGroup_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var cg = _seeder.Add(TestData.GenerateRandomConsumerGroup());
            var cgDto = new ConsumerGroupBuilder().WithUri(cg.Uri).BuildDto();

            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/defaultConsumerGroup", BuildJsonHttpContent(cgDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDefaultConsumerGroup_Put_Returns_BadRequest_IfConsumerGroupDoesNotExist()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var invalidCGUri = $"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultConsumerGroup", BuildJsonHttpContent(invalidCGUri));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDefaultConsumerGroup_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultConsumerGroup", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDefaultConsumerGroup_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultConsumerGroup", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Default Consumer Group | /api/users/{userId}/defaultConsumerGroup

        #region Last Login Editor | /api/users/{userId}/lastLoginEditor

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var originalLastLoginEditor = user.LastLoginEditor;
            var now = DateTime.UtcNow;

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginEditor", BuildJsonHttpContent(now));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.NotEqual(originalLastLoginEditor, responseUser.LastLoginEditor);
            Assert.Equal(now, responseUser.LastLoginEditor);
        }

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/lastLoginEditor", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.PutAsync($"{PATH}/somefancyuser/lastLoginEditor", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_BadRequest_IfContentIsNoTimestamp()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var somestring = "Yep, this is just wrong";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginEditor", BuildPlainTextHttpContent(somestring));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginEditor", BuildPlainTextHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginEditor_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginEditor", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Last Login Editor | /api/users/{userId}/lastLoginEditor

        #region Last Login Data Marketplace | /api/users/{userId}/lastLoginDataMarketplace

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var originalLastLoginDataMarketplace = user.LastLoginDataMarketplace;
            var now = DateTime.UtcNow;

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginDataMarketplace", BuildJsonHttpContent(now));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.NotEqual(originalLastLoginDataMarketplace, responseUser.LastLoginDataMarketplace);
            Assert.Equal(now, responseUser.LastLoginDataMarketplace);
        }

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/lastLoginDataMarketplace", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.PutAsync($"{PATH}/somefancyuser/lastLoginDataMarketplace", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_BadRequest_IfContentIsNoTimestamp()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var somestring = "Yep, this is just wrong";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginDataMarketplace", BuildPlainTextHttpContent(somestring));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginDataMarketplace", BuildPlainTextHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastLoginDataMarketplace_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastLoginDataMarketplace", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Last Login Data Marketplace | /api/users/{userId}/lastLoginDataMarketplace

        #region Last Time Checked | /api/users/{userId}/lastTimeChecked

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var originalLastTimeChecked = user.LastTimeChecked;
            var now = DateTime.UtcNow;

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastTimeChecked", BuildJsonHttpContent(now));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.NotEqual(originalLastTimeChecked, responseUser.LastTimeChecked);
            Assert.Equal(now, responseUser.LastTimeChecked);
        }

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/lastTimeChecked", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.PutAsync($"{PATH}/somefancyuser/lastTimeChecked", BuildJsonHttpContent(DateTime.UtcNow));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_BadRequest_IfContentIsNoTimestamp()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var somestring = "Yep, this is just wrong";

            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastTimeChecked", BuildPlainTextHttpContent(somestring));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastTimeChecked", BuildPlainTextHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLastTimeChecked_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/lastTimeChecked", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Last Time Checked | /api/users/{userId}/lastTimeChecked

        #region Search Filters Editor | /api/users/{userId}/searchFilterEditor/*

        [Fact]
        public async Task SearchFilterEditor_Get_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterEditor());
            user = _seeder.AppendSearchFilterEditorToUser(sf, user);

            var response = await Client.GetAsync($"{PATH}/{user.Id}/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseFilters = JsonConvert.DeserializeObject<SearchFilterEditor>(stringResponse);

            Assert.NotNull(responseFilters);
            Assert.Equal(sf.ToString(), responseFilters.ToString());
        }

        [Fact]
        public async Task SearchFilterEditor_Get_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Get_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/somefancyid/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfJObject = TestData.GenerateRandomSearchFilterEditor().FilterJson;

            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFilterEditor", BuildJsonHttpContent(sfJObject));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotNull(responseUser.SearchFilterEditor);

            var responseSf = responseUser.SearchFilterEditor;
            Assert.Equal(sfJObject, responseSf.FilterJson);
            Assert.NotEqual(0, responseSf.Id);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var sfJObject = TestData.GenerateRandomSearchFilterEditor().FilterJson;
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/searchFilterEditor", BuildJsonHttpContent(sfJObject));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var sfJObject = TestData.GenerateRandomSearchFilterEditor().FilterJson;
            var response = await Client.PutAsync($"{PATH}/somefancyid/searchFilterEditor", BuildJsonHttpContent(sfJObject));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFilterEditor", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFilterEditor", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Put_Returns_UnsupportedMediaType_IfContentIsPlainText()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFilterEditor", BuildPlainTextHttpContent("123"));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Delete_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterEditor());
            user = _seeder.AppendSearchFilterEditorToUser(sf, user);

            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Delete_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.DeleteAsync($"{PATH}/{Guid.NewGuid()}/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterEditor_Delete_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.DeleteAsync($"{PATH}/somefancyguid/searchFilterEditor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Search Filters Editor | /api/users/{userId}/searchFilterEditor/*

        #region Default Search Filters Data Marketplace | /api/users/{userId}/defaultSearchFilterDataMarketplace

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Get_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf, user);
            user = _seeder.SetDefaultSearchFilterDataMarketplaceToUser(sf.Id, user);

            var response = await Client.GetAsync($"{PATH}/{user.Id}/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseFilter = JsonConvert.DeserializeObject<SearchFilterDataMarketplace>(stringResponse);

            Assert.NotNull(responseFilter);
            Assert.Equal(sf.Id, responseFilter.Id);
            Assert.Equal(sf.Name, responseFilter.Name);
            Assert.Equal(sf.FilterJson, responseFilter.FilterJson);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Get_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Get_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/somefancyid/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf, user);
            Assert.Null(user.DefaultSearchFilterDataMarketplace);

            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultSearchFilterDataMarketplace/{sf.Id}", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotNull(responseUser.DefaultSearchFilterDataMarketplace);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var searchFilterId = 1;
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/defaultSearchFilterDataMarketplace/{searchFilterId}", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var searchFilterId = 1;
            var response = await Client.PutAsync($"{PATH}/somefancyid/defaultSearchFilterDataMarketplace/{searchFilterId}", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Put_Returns_NotFound_IfSearchFilterIdNotExists()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf, user);
            user = _seeder.SetDefaultSearchFilterDataMarketplaceToUser(sf.Id, user);
            Assert.NotNull(user.DefaultSearchFilterDataMarketplace);

            var invalidSearchFilterId = 1337;
            var response = await Client.PutAsync($"{PATH}/{user.Id}/defaultSearchFilterDataMarketplace/{invalidSearchFilterId}", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Delete_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf1 = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf1, user);
            user = _seeder.SetDefaultSearchFilterDataMarketplaceToUser(sf1.Id, user);
            Assert.NotNull(user.DefaultSearchFilterDataMarketplace);

            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Delete_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.DeleteAsync($"{PATH}/{Guid.NewGuid()}/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DefaultSearchFilterDataMarketplace_Delete_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.DeleteAsync($"{PATH}/somefancyguid/defaultSearchFilterDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Default Search Filters Data Marketplace | /api/users/{userId}/defaultSearchFilterDataMarketplace

        #region Search Filters Data Marketplace | /api/users/{userId}/searchFilterDataMarketplace/*

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_All_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf1 = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            var sf2 = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf1, user);
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf2, user);

            var response = await Client.GetAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseFilters = JsonConvert.DeserializeObject<List<SearchFilterDataMarketplace>>(stringResponse);

            Assert.NotNull(responseFilters);
            Assert.NotEmpty(responseFilters);
            Assert.Equal(2, responseFilters.Count);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_All_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/searchFiltersDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_All_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/somefancyid/searchFiltersDataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_Single_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf, user);

            var response = await Client.GetAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sf.Id}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseFilters = JsonConvert.DeserializeObject<SearchFilterDataMarketplace>(stringResponse);
            Assert.NotNull(responseFilters);
            Assert.Equal(sf.ToString(), responseFilters.ToString());
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_Single_Returns_NotFound_IfUserDoesNotExist()
        {
            var sfId = 10;
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_Single_Returns_NotFound_IfSearchFilterDoesNotExist()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfId = 1337;
            var response = await Client.GetAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_Single_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var sfId = 10;
            var response = await Client.GetAsync($"{PATH}/somefancyguid/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Get_Single_Returns_BadRequest_IfSearchFilterIdIsNoInteger()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfId = "abc";
            var response = await Client.GetAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfDto = _mapper.Map<SearchFilterDataMarketplaceDto>(TestData.GenerateRandomSearchFilterDataMarketplace());

            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace", BuildJsonHttpContent(sfDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEmpty(responseUser.SearchFiltersDataMarketplace);
            Assert.Single(responseUser.SearchFiltersDataMarketplace);

            var responseSf = responseUser.SearchFiltersDataMarketplace.First();
            Assert.Equal(sfDto.Name, responseSf.Name);
            Assert.Equal(sfDto.FilterJson, responseSf.FilterJson);
            Assert.NotEqual(0, responseSf.Id);

            // Check if entity is REALLY added to the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.NotNull(verifyUser.SearchFiltersDataMarketplace);
            Assert.True(verifyUser.SearchFiltersDataMarketplace.Any());
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var sfDto = _mapper.Map<SearchFilterDataMarketplaceDto>(TestData.GenerateRandomSearchFilterDataMarketplace());
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/searchFiltersDataMarketplace", BuildJsonHttpContent(sfDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var sfDto = _mapper.Map<SearchFilterDataMarketplaceDto>(TestData.GenerateRandomSearchFilterDataMarketplace());
            var response = await Client.PutAsync($"{PATH}/somefancyid/searchFiltersDataMarketplace", BuildJsonHttpContent(sfDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Put_Returns_UnsupportedMediaType_IfContentIsPlainText()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace", BuildPlainTextHttpContent("123"));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Delete_Single_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sf = _seeder.Add(TestData.GenerateRandomSearchFilterDataMarketplace());
            user = _seeder.AppendSearchFilterDataMarketplaceToUser(sf, user);

            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sf.Id}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.True(verifyUser.SearchFiltersDataMarketplace == null || !verifyUser.SearchFiltersDataMarketplace.Any());
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Delete_Single_Returns_NotFound_UserDoesNotExist()
        {
            var sfId = 10;
            var response = await Client.DeleteAsync($"{PATH}/{Guid.NewGuid()}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Delete_Single_Returns_NotFound_SearchFilterIdDoesNotExist()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfId = 10;
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Delete_Single_Returns_BadRequest_UserIdIsNoGuid()
        {
            var sfId = 10;
            var response = await Client.DeleteAsync($"{PATH}/somefancyguid/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFilterDataMarketplace_Delete_Single_Returns_BadRequest_SearchFilterIdIsNoInteger()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sfId = "abc";
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/searchFiltersDataMarketplace/{sfId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion Search Filters Data Marketplace | /api/users/{userId}/searchFilterDataMarketplace/*

        #region Colid Entry Subscriptions | /api/users/{userId}/colidEntrySubscriptions/*

        [Fact]
        public async Task ColidEntrySubscriptions_Get_All_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var ce1 = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var ce2 = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var sub1 = _seeder.AppendColidEntrySubscriptionToUser(ce1, user);
            var sub2 = _seeder.AppendColidEntrySubscriptionToUser(ce2, user);

            var response = await Client.GetAsync($"{PATH}/{user.Id}/colidEntrySubscriptions");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseFilters = JsonConvert.DeserializeObject<List<ColidEntrySubscriptionDto>>(stringResponse);

            Assert.NotNull(responseFilters);
            Assert.NotEmpty(responseFilters);
            Assert.Equal(2, responseFilters.Count);
            responseFilters.ForEach(e => Assert.NotNull(e.ColidPidUri));
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Get_All_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/colidEntrySubscriptions");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Get_All_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/somefancyid/colidEntrySubscriptions");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var ceDto = _mapper.Map<ColidEntrySubscriptionDto>(TestData.GenerateRandomColidEntrySubscriptionDto());

            var response = await Client.PutAsync($"{PATH}/{user.Id}/colidEntrySubscriptions", BuildJsonHttpContent(ceDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEmpty(responseUser.ColidEntrySubscriptions);
            Assert.Single(responseUser.ColidEntrySubscriptions);

            var responseCE = responseUser.ColidEntrySubscriptions.First();
            Assert.Equal(ceDto.ColidPidUri.AbsoluteUri, responseCE.ColidPidUri.AbsoluteUri);
            Assert.NotEqual(0, responseCE.Id);

            // Check if entity is REALLY added to the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.NotNull(verifyUser.ColidEntrySubscriptions);
            Assert.True(verifyUser.ColidEntrySubscriptions.Any());
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_BadRequest_IfAlreadySubscribed()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var ces = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var subscription = _seeder.AppendColidEntrySubscriptionToUser(ces, user);

            var ceDto = _mapper.Map<ColidEntrySubscriptionDto>(ces);

            var response = await Client.PutAsync($"{PATH}/{user.Id}/colidEntrySubscriptions", BuildJsonHttpContent(ceDto));
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_NotFound_IfUserDoesNotExist()
        {
            var ceDto = _mapper.Map<ColidEntrySubscriptionDto>(TestData.GenerateRandomColidEntrySubscriptionDto());
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/colidEntrySubscriptions", BuildJsonHttpContent(ceDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var ceDto = _mapper.Map<ColidEntrySubscriptionDto>(TestData.GenerateRandomColidEntrySubscriptionDto());
            var response = await Client.PutAsync($"{PATH}/somefancyid/colidEntrySubscriptions", BuildJsonHttpContent(ceDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/colidEntrySubscriptions", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/colidEntrySubscriptions", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Put_Returns_UnsupportedMediaType_IfContentIsPlainText()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/colidEntrySubscriptions", BuildPlainTextHttpContent("123"));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Delete_Single_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var ceEntity = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var ceDto = new ColidEntrySubscriptionBuilder().WithColidEntry(ceEntity.ColidPidUri).BuildDto();
            var subscription = _seeder.AppendColidEntrySubscriptionToUser(ceEntity, user);

            var response = await _api.SendDeleteRequestWithContent($"{PATH}/{user.Id}/colidEntrySubscriptions", _api.BuildJsonHttpContent(ceDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.True(verifyUser.ColidEntrySubscriptions == null || !verifyUser.ColidEntrySubscriptions.Any());
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Delete_Single_Returns_NotFound_ColidEntrySubscriptionDoesNotExist()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var nonExistingCe = new ColidEntrySubscriptionBuilder().WithColidEntry(new Uri("http://incredibleUri2020/")).BuildDto();
            await _api.DeleteWithContent_ShouldReturn_NotFound_IfEntityDoesNotExist($"{PATH}/{user.Id}/colidEntrySubscriptions", _api.BuildJsonHttpContent(nonExistingCe));
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Delete_Single_Returns_BadRequest_UserIdIsNoGuid()
        {
            var ceEntity = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var ceDto = new ColidEntrySubscriptionBuilder().WithColidEntry(ceEntity.ColidPidUri).BuildDto();
            await _api.DeleteWithContent_ShouldReturn_BadRequest_IfIdHasTypeMismatch($"{PATH}/somefancyguid/colidEntrySubscriptions", _api.BuildJsonHttpContent(ceDto));
        }

        [Fact]
        public async Task ColidEntrySubscriptions_Delete_Single_Returns_BadRequest_ColidEntrySubscriptionIsNoDto()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            await _api.DeleteWithContent_ShouldReturn_BadRequest_IfContentHasTypeMismatch($"{PATH}/{user.Id}/colidEntrySubscriptions", _api.BuildJsonHttpContent("123"));
        }

        #endregion Colid Entry Subscriptions | /api/users/{userId}/colidEntrySubscriptions/*

        #region Message Config | /api/users/{userId}/messageConfig/*
        [Fact]
        public async Task MessageConfig_Get_Single_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());

            var response = await Client.GetAsync($"{PATH}/{user.Id}/messageConfig");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseMessageConfig = JsonConvert.DeserializeObject<MessageConfig>(stringResponse);
            Assert.NotNull(responseMessageConfig);
            Assert.Equal(user.MessageConfig.SendInterval, responseMessageConfig.SendInterval);
            Assert.Equal(user.MessageConfig.DeleteInterval, responseMessageConfig.DeleteInterval);
        }

        [Fact]
        public async Task MessageConfig_Get_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/messageConfig");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_Ok()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var originalSendInterval = user.MessageConfig.SendInterval;
            var updatedSendInterval = SendInterval.Daily;
            var newMessageConfig = new MessageConfigBuilder().WithSendInterval(SendInterval.Daily).WithDeleteInterval(user.MessageConfig.DeleteInterval).Build();
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(newMessageConfig));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEqual(originalSendInterval, responseUser.MessageConfig.SendInterval);
            Assert.Equal(updatedSendInterval, responseUser.MessageConfig.SendInterval);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_Ok_IfNothingChanged()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var sameMessageConfig = new MessageConfigBuilder().WithSendInterval(SendInterval.Weekly).WithDeleteInterval(DeleteInterval.Monthly).Build();
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(sameMessageConfig));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_Ok_WithUpdatedMessage()
        {
            var message = TestData.GenerateSampleMessage();
            var originalSendOn = message.SendOn;
            var originalDeleteOn = message.DeleteOn;
            _output.WriteLine("Message:" + message);

            var user = TestData.GenerateRandomUser(); // default weekly & Monthly
            user.Messages = new List<Message>() { message };
            user = _seeder.Add(user);

            var newMessageConfig = new MessageConfigBuilder().WithSendInterval(SendInterval.Daily).WithDeleteInterval(DeleteInterval.Weekly).Build();
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(newMessageConfig));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(responseUser);
            Assert.NotEqual(originalSendOn, responseUser.Messages.FirstOrDefault().SendOn);
            Assert.NotEqual(originalDeleteOn, responseUser.Messages.FirstOrDefault().DeleteOn);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_BadRequest_IfContentIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(""));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_BadRequest_IfSendSameAsDelete()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var invalidMessageConfig = new MessageConfigBuilder().WithSendInterval(SendInterval.Monthly).WithDeleteInterval(DeleteInterval.Monthly).Build();
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(invalidMessageConfig));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMessageConfig_Put_Returns_BadRequest_IfSendHigherThanDelete()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var invalidMessageConfig = new MessageConfigBuilder().WithSendInterval(SendInterval.Monthly).WithDeleteInterval(DeleteInterval.Weekly).Build();
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messageConfig", BuildJsonHttpContent(invalidMessageConfig));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Message | /api/users/{userId}/messages

        [Fact]
        public async Task Messages_Get_All_Returns_Ok()
        {
            var seedUser = TestData.GenerateRandomUser();
            seedUser.Messages = new List<Message> { TestData.GenerateSampleMessage(), TestData.GenerateSampleMessage() };
            var user = _seeder.Add(seedUser);
            var response = await Client.GetAsync($"{PATH}/{user.Id}/messages");
            var stringResponse = await response.Content.ReadAsStringAsync();

            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var messages = JsonConvert.DeserializeObject<List<MessageDto>>(stringResponse);
            Assert.Equal(seedUser.Messages.Count, messages.Count());
        }

        [Fact]
        public async Task Messages_Get_All_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.GetAsync($"{PATH}/{Guid.NewGuid()}/messages");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Messages_Get_All_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.GetAsync($"{PATH}/no-guid/messages");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Messages_Delete_Single_Returns_Ok()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();
            var seedMessage = TestData.GenerateSampleMessage();
            seedMessage.Id = 20;
            seedUser.Messages = new List<Message> { seedMessage };
            var user = _seeder.Add(seedUser);

            // Act
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/messages/{seedMessage.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.True(verifyUser.Messages == null || !verifyUser.Messages.Any());
        }

        [Fact]
        public async Task Message_Delete_Single_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.DeleteAsync($"{PATH}/no-guid/messages/{_rnd.Next()}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Message_Delete_Single_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.DeleteAsync($"{PATH}/{Guid.NewGuid()}/messages/{_rnd.Next()}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Message_Delete_Single_Returns_NotFound_IfMessageDoesNotExist()
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/messages/{_rnd.Next()}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Message_Delete_Single_Returns_BadRequest_IfMessageIdIsNoPositiveInteger(int messageId)
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.DeleteAsync($"{PATH}/{user.Id}/messages/{messageId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Messages_MarkRead_Returns_Ok()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();

            var seedMessageOne = TestData.GenerateSampleMessage();
            seedMessageOne.Id = 3;

            var seedMessageTwo = TestData.GenerateSampleMessage();
            seedMessageTwo.Id = 4;

            seedUser.Messages = new List<Message> { seedMessageOne, seedMessageTwo };
            var user = _seeder.Add(seedUser);
            var messageIds = new List<int> { 3, 4 };

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/markRead", BuildJsonHttpContent(messageIds));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.Equal(2, verifyUser.Messages.Count);
            Assert.All(verifyUser.Messages, m =>
            {
                Assert.NotNull(m.ReadOn);
                Assert.Null(m.SendOn);
            });
            Assert.Single(verifyUser.Messages, m => m.Id == seedMessageOne.Id);
            Assert.Single(verifyUser.Messages, m => m.Id == seedMessageTwo.Id);
        }

        [Fact]
        public async Task Messages_MarkRead_Returns_NotFound_IfUserDoesNotExist()
        {
            var messageIds = new List<int> { 1, 2 };
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/messages/markRead", BuildJsonHttpContent(messageIds));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Messages_MarkRead_Returns_NotFound_IfUserIdIsNoGuid()
        {
            var messageIds = new List<int> { 1, 2 };
            var response = await Client.PutAsync($"{PATH}/no-guid/messages/markRead", BuildJsonHttpContent(messageIds));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Messages_MarkRead_Returns_Ok_IgnoreMessagesNotExists()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();

            var seedMessageOne = TestData.GenerateSampleMessage();
            seedMessageOne.Id = 5;
            var seedMessageTwo = TestData.GenerateSampleMessage();
            seedMessageTwo.Id = 6;

            seedUser.Messages = new List<Message> { seedMessageOne, seedMessageTwo };
            var user = _seeder.Add(seedUser);
            var messageIds = new List<int> { 1, 2, 3, 4, 5, 6 };

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/markRead", BuildJsonHttpContent(messageIds));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.Equal(2, verifyUser.Messages.Count);
            Assert.All(verifyUser.Messages, m =>
            {
                Assert.NotNull(m.ReadOn);
                Assert.Null(m.SendOn);
            });
        }

        [Fact]
        public async Task Messages_MarkRead_Returns_BadRequest_IfListContainsInvalidIds()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var messageIds = new List<int> { -1, 0 };

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/markRead", BuildJsonHttpContent(messageIds));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Messages_MarkRead_Single_Returns_Ok()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();
            var seedMessage = TestData.GenerateSampleMessage();
            seedMessage.Id = 7;
            seedMessage.ReadOn = null;
            seedMessage.SendOn = null;
            seedUser.Messages = new List<Message> { seedMessage };
            var user = _seeder.Add(seedUser);

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{seedMessage.Id}/markRead", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.Single(verifyUser.Messages);
            Assert.Single(verifyUser.Messages, m => m.Id == seedMessage.Id && m.ReadOn != null);
        }

        [Fact]
        public async Task Message_MarkRead_Single_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.PutAsync($"{PATH}/no-guid/messages/{_rnd.Next()}/markRead", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Message_MarkRead_Single_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/messages/{_rnd.Next()}/markRead", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Message_MarkRead_Single_Returns_NotFound_IfMessageDoesNotExist()
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{_rnd.Next()}/markRead", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Message_MarkRead_Single_Returns_BadRequest_IfMessageIdIsNoPositiveInteger(int messageId)
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{messageId}/markRead", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Messages_MarkSent_Single_Returns_Ok()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();
            var seedMessage = TestData.GenerateSampleMessage();
            seedMessage.Id = 8;
            seedUser.Messages = new List<Message> { seedMessage };
            var user = _seeder.Add(seedUser);

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{seedMessage.Id}/markSent", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.Single(verifyUser.Messages);
            Assert.Single(verifyUser.Messages, m => m.Id == seedMessage.Id && m.ReadOn != null && m.SendOn == null);
        }

        [Fact]
        public async Task Messages_MarkSent_Single_Returns_Ok_IfMessageAlreadySent()
        {
            // Arrange
            var seedUser = TestData.GenerateRandomUser();
            var seedMessage = TestData.GenerateSampleMessage();
            seedMessage.Id = 9;
            seedMessage.SendOn = null;

            seedUser.Messages = new List<Message> { seedMessage };
            var user = _seeder.Add(seedUser);

            // Act
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{seedMessage.Id}/markSent", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check if entity is REALLY removed from the user
            var verifyUser = await _api.GetUser(user.Id);
            Assert.Single(verifyUser.Messages);
            Assert.Single(verifyUser.Messages, m => m.Id == seedMessage.Id && m.ReadOn == seedMessage.ReadOn && m.SendOn == null);
        }

        [Fact]
        public async Task Message_MarkSent_Single_Returns_BadRequest_IfUserIdIsNoGuid()
        {
            var response = await Client.PutAsync($"{PATH}/no-guid/messages/{_rnd.Next()}/markSent", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Message_MarkSent_Single_Returns_NotFound_IfUserDoesNotExist()
        {
            var response = await Client.PutAsync($"{PATH}/{Guid.NewGuid()}/messages/{_rnd.Next()}/markSent", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Message_MarkSent_Single_Returns_NotFound_IfMessageDoesNotExist()
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{_rnd.Next()}/markSent", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Message_MarkSent_Single_Returns_BadRequest_IfMessageIdIsNoPositiveInteger(int messageId)
        {
            var seedUser = TestData.GenerateRandomUser();
            var user = _seeder.Add(seedUser);
            var response = await Client.PutAsync($"{PATH}/{user.Id}/messages/{messageId}/markSent", null);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Helper methods

        private static HttpContent BuildJsonHttpContent<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpContent requestContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            return requestContent;
        }

        private static HttpContent BuildPlainTextHttpContent<T>(T obj)
        {
            return new StringContent(obj.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain);
        }

        #endregion Helper methods
    }
}
