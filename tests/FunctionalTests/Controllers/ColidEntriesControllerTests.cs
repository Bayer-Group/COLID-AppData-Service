using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class ColidEntriesControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/colidEntries";

        private readonly TestDataContextSeeder _seeder;
        private readonly ApiTestHelper _api;
        private readonly ITestOutputHelper _output;

        public ColidEntriesControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;
        }

        [Fact]
        public async Task NotifyUpdatedColidEntryToSubscribers_Put_Returns_Ok()
        {
            _seeder.ClearUsers();
            _seeder.SeedMessageTemplates();

            // IMPORTANT: User needs to be new, and not persisted already, even if it doesn't make sense ...
            // thats the reason userEntity is commented. IF you add it before the subscription, another user will be tried
            // so add. Don't ask me why .. Alternatively you can add a new subscription to the user before creation:
            /*
                var template = TestData.GenerateSampleMessageTemplateForColidEntrySubscriptionUpdate();
                var templateEntity = _seeder.Add(template);
                var subscription = TestData.GenerateRandomColidEntrySubscription();
                var user = TestData.GenerateRandomUser();
                user.ColidEntrySubscriptions = new Collection<ColidEntrySubscription>() { subscription };
                var userEntity = _seeder.Add(user);
            */

            var user = TestData.GenerateRandomUser();
            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            _seeder.Add(subscription);
            var colidEntryDto = new ColidEntryDtoBuilder().WithColidPidUri(subscription.ColidPidUri).WithLabel("Hugo Boss Maximum fragrance").Build();

            var response = await Client.PutAsync($"{PATH}", _api.BuildJsonHttpContent(colidEntryDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            Assert.Contains("1", stringResponse);
        }

        [Fact]
        public async Task NotifyUpdatedColidEntryToSubscribers_Put_Returns_BadRequest_IfContentIsEmpty()
        {
            await _api.Put_ShouldReturn_BadRequest_IfJsonContentIsEmpty(PATH);
        }

        [Fact]
        public async Task NotifyUpdatedColidEntryToSubscribers_Put_Returns_BadRequest_IfContentIsNull()
        {
            await _api.Put_ShouldReturn_BadRequest_IfContentIsNull(PATH);
        }
    }
}
