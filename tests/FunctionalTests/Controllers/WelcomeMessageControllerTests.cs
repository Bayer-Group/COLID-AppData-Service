using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class WelcomeMessageControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/welcomeMessages";

        private readonly TestDataContextSeeder _seeder;
        private readonly ApiTestHelper _api;
        private readonly ITestOutputHelper _output;

        private readonly WelcomeMessage _welcomeMessageEditor;
        private readonly WelcomeMessage _welcomeMessageDataMarketplace;

        public WelcomeMessageControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;

            _welcomeMessageEditor = TestData.GetPreconfiguredWelcomeMessages().Where(e => e.Type.Equals(ColidType.Editor)).First();
            _welcomeMessageDataMarketplace = TestData.GetPreconfiguredWelcomeMessages().Where(e => e.Type.Equals(ColidType.DataMarketplace)).First();
        }

        [Fact]
        public async Task WelcomeMessageEditor_Get_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/editor");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<WelcomeMessage>(stringResponse);

            Assert.NotNull(responseMessage);
            Assert.Equal(_welcomeMessageEditor.Content, responseMessage.Content);

            _output.WriteLine(responseMessage.Content);
        }

        [Fact]
        public async Task WelcomeMessageEditor_Update_Returns_Ok()
        {
            var originalContent = _welcomeMessageEditor.Content;
            var updatedContent = "<h1>this is the new content</h1>";

            var response = await Client.PutAsync($"{PATH}/editor", _api.BuildJsonHttpContent(updatedContent));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<WelcomeMessage>(stringResponse);

            Assert.NotNull(responseMessage);
            Assert.NotEqual(originalContent, responseMessage.Content);
            Assert.Equal(updatedContent, responseMessage.Content);

            _seeder.ResetWelcomeMessages();
        }

        [Fact]
        public async Task WelcomeMessageDataMarketplace_Get_Returns_Ok()
        {
            var response = await Client.GetAsync($"{PATH}/dataMarketplace");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<WelcomeMessage>(stringResponse);

            Assert.NotNull(responseMessage);
            Assert.Equal(_welcomeMessageDataMarketplace.Content, responseMessage.Content);

            _output.WriteLine(responseMessage.Content);
        }

        [Fact]
        public async Task WelcomeMessageDataMarketplace_Update_Returns_Ok()
        {
            var originalContent = _welcomeMessageDataMarketplace.Content;
            var updatedContent = "<h1>this is the new content</h1>";

            var response = await Client.PutAsync($"{PATH}/dataMarketplace", _api.BuildJsonHttpContent(updatedContent));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<WelcomeMessage>(stringResponse);

            Assert.NotNull(responseMessage);
            Assert.NotEqual(originalContent, responseMessage.Content);
            Assert.Equal(updatedContent, responseMessage.Content);

            _seeder.ResetWelcomeMessages();
        }
    }
}
