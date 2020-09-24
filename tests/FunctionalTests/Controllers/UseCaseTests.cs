using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class UseCaseTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string USER_PATH = "/api/users";
        private const string CG_PATH = "/api/consumerGroups";

        private readonly TestDataContextSeeder _seeder;

        private readonly ITestOutputHelper _output;

        private readonly IEnumerable<User> _userList = TestData.GetPreconfiguredUsers();
        private readonly IEnumerable<ConsumerGroup> _consumerGroupList = TestData.GetPreconfiguredConsumerGroups();

        public UseCaseTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _output = output;
        }

        [Fact]
        public async Task CreateUser_CreateCg_AssignCg_DeleteCg()
        {
            var userId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var cgUri = new Uri("http://superspecialconsumergroup#1233213#554");

            await CreateUser(userId);
            await CreateCg(cgUri);
            await AssignCgToUser(cgUri, userId);
            await DeleteCG(cgUri);
            await UserShouldntHaveCg(userId);
        }

        private async Task CreateUser(Guid userId)
        {
            var email = "john.finlay@tiger.king";
            var user = new UserBuilder().WithId(userId).WithEmailAddress(email).Build();
            HttpContent requestContent = new StringContent(user.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await Client.PostAsync(USER_PATH, requestContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("CreateUser: " + stringResponse + "\n");
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.Equal(userId, responseUser.Id);
            Assert.Equal(email, responseUser.EmailAddress);
        }

        private async Task CreateCg(Uri cgUri)
        {
            var cgToCreate = new ConsumerGroupBuilder().WithUri(cgUri).BuildDto();
            var requestContent = BuildJsonHttpContent(cgToCreate);

            var response = await Client.PostAsync(CG_PATH, requestContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("CreateCG: " + stringResponse + "\n");
            response.EnsureSuccessStatusCode();

            var cgs = JsonConvert.DeserializeObject<ConsumerGroup>(stringResponse);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(cgs);
            Assert.NotEqual(0, cgs.Id);
            Assert.Equal(cgUri, cgs.Uri);
        }

        private async Task AssignCgToUser(Uri cgUri, Guid userId)
        {
            var cgDto = new ConsumerGroupBuilder().WithUri(cgUri).BuildDto();
            var requestContent = BuildJsonHttpContent(cgDto);

            var response = await Client.PutAsync($"{USER_PATH}/{userId}/defaultConsumerGroup", requestContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("AssignCgToUser: " + stringResponse + "\n");
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.Equal(cgUri.AbsoluteUri, responseUser.DefaultConsumerGroup.Uri.AbsoluteUri);

            var x = await getUser(userId);
            Assert.NotNull(x.DefaultConsumerGroup);
        }

        private async Task DeleteCG(Uri cgUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, CG_PATH);
            var cgToDelete = new ConsumerGroupBuilder().WithUri(cgUri).BuildDto();
            request.Content = BuildJsonHttpContent(cgToDelete);

            var deleteResponse = await Client.SendAsync(request);
            var deleteResponseContent = await deleteResponse.Content.ReadAsStringAsync();
            _output.WriteLine(deleteResponseContent);
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse = await Client.GetAsync(CG_PATH);
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var foundCgs = JsonConvert.DeserializeObject<List<ConsumerGroup>>(getResponseContent);

            _output.WriteLine("Found CGs: " + string.Join(", ", foundCgs));

            var cgShouldBeNull = foundCgs.SingleOrDefault(e => Uri.Compare(e.Uri, cgUri, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.InvariantCulture) == 0);

            _output.WriteLine("cgShouldBeNull: " + cgShouldBeNull);
            Assert.Null(cgShouldBeNull);
        }

        private async Task UserShouldntHaveCg(Guid userId)
        {
            var response = await Client.GetAsync($"{USER_PATH}/{userId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Found User: " + stringResponse + "\n\n");
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.Null(responseUser.DefaultConsumerGroup);
        }

        // ============================================================

        private async Task<User> getUser(Guid userId)
        {
            var response = await Client.GetAsync($"{USER_PATH}/{userId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Found User: " + stringResponse + "\n\n");
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            return responseUser;
        }

        private static HttpContent BuildJsonHttpContent<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpContent requestContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            return requestContent;
        }
    }
}
