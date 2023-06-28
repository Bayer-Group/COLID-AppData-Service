using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class ActiveDirectoryControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/activeDirectory";

        private readonly ITestOutputHelper _output;

        private readonly ApiTestHelper _api;

        private const string ValidEmail = "demo.user@bayer.com";

        public ActiveDirectoryControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;
        }

        [Fact]
        public async Task Get_FindUsers_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/users?q=bayer");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adUsers = await _api.DeserializeHttpContent<List<AdUser>>(response);

            Assert.NotNull(adUsers);
            Assert.NotEmpty(adUsers);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Get_FindUsers_Returns_BadRequest_IfQueryIsInvalid(string query)
        {
            var response = await Client.GetAsync($"{PATH}/users?q={query}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_GetUser_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/users/{ValidEmail}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adUser = await _api.DeserializeHttpContent<AdUser>(response);

            Assert.NotNull(adUser);
            Assert.Equal(ValidEmail, adUser.Id);
        }

        [Theory]
        [InlineData("invalid-id")]
        [InlineData("2180db5d-79ae-4204-8b2b-b4665928a9b2-12")]
        [InlineData("wrong.mail@bayer@.de")]
        public async Task Get_GetUser_Returns_BadRequest_IfArgumentIsInvalid(string arg)
        {
            var response = await Client.GetAsync($"{PATH}/users/{arg}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_GetUserManager_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/users/{ValidEmail}/manager");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adUser = await _api.DeserializeHttpContent<AdUser>(response);

            Assert.NotNull(adUser);
            Assert.Equal(ValidEmail, adUser.Id);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("invalid-id")]
        [InlineData("2180db5d-79ae-4204-8b2b-b4665928a9b2-12")]
        [InlineData("wrong.mail@bayer@.de")]
        public async Task Get_GetUserManager_Returns_BadRequest_IfArgumentIsInvalid(string arg)
        {
            var response = await Client.GetAsync($"{PATH}/users/{arg}/manager");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_FindGroups_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/groups?q=bayer");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adGroups = await _api.DeserializeHttpContent<List<AdGroup>>(response);

            Assert.NotNull(adGroups);
            Assert.NotEmpty(adGroups);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Get_FindGroups_Returns_BadRequest_IfArgumentIsInvalid(string query)
        {
            var response = await Client.GetAsync($"{PATH}/groups?q={query}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_GetGroup_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/groups/{ValidEmail}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adGroup = await _api.DeserializeHttpContent<AdGroup>(response);

            Assert.NotNull(adGroup);
            Assert.Equal(ValidEmail, adGroup.Id);
        }

        [Theory]
        [InlineData("invalid-id")]
        [InlineData("2180db5d-79ae-4204-8b2b-b4665928a9b2")]
        [InlineData("2180db5d-79ae-4204-8b2b-b4665928a9b2-12")]
        [InlineData("wrong.mail@bayer@.de")]
        public async Task Get_GetGroup_Returns_BadRequest_IfArgumentIsInvalid(string arg)
        {
            var response = await Client.GetAsync($"{PATH}/groups/{arg}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_FindUserAndGroups_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/usersAndGroups?q=bayer");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adSearchResult = await _api.DeserializeHttpContent<AdSearchResult>(response);

            Assert.NotNull(adSearchResult);
            Assert.NotEmpty(adSearchResult.Groups);
            Assert.NotEmpty(adSearchResult.Users);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Get_FindUserAndGroups_Returns_BadRequest_IfQueryIsInvalid(string query)
        {
            var response = await Client.GetAsync($"{PATH}/usersAndGroups?q={query}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_GetUserOrGroup_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/usersAndGroups/{ValidEmail}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);

            var adObject = await _api.DeserializeHttpContent<AdObject>(response);

            Assert.NotNull(adObject);
            Assert.Equal(ValidEmail, adObject.Id);
        }

        [Theory]
        [InlineData("invalid-id")]
        [InlineData("2180db5d-79ae-4204-8b2b-b4665928a9b2-12")]
        [InlineData("wrong.mail@bayer@.de")]
        public async Task Get_GetUserOrGroup_Returns_BadRequest_IfArgumentIsInvalid(string arg)
        {
            var response = await Client.GetAsync($"{PATH}/usersAndGroups/{arg}");
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }
    }
}
