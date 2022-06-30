using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Newtonsoft.Json;
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

        [Fact]
        public async Task GetAmountOfSubscribers_Post_OneUser_OneSubscribedPidUri_Returns_NumberOfSubscribers()
        {
            _seeder.ClearUsers();
            _seeder.SeedMessageTemplates();

            var user = TestData.GenerateRandomUser();
            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            _seeder.Add(subscription);

            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent(new HashSet<Uri>() { subscription.ColidPidUri }));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ColidEntrySubscriptionAmountDto>>(stringResponse);

            Assert.Single(result);
            Assert.Equal(subscription.ColidPidUri, result.First().ColidPidUri);
            Assert.Equal(1, result.First().Subscriptions);
        }

        [Fact]
        public async Task GetAmountOfSubscribers_Post_TwoUsers_SameSubscribedPidUri_Returns_NumberOfSubscribers()
        {
            _seeder.ClearUsers();
            _seeder.SeedMessageTemplates();

            var user = TestData.GenerateRandomUser();
            var user2 = TestData.GenerateRandomUser();

            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            var subscription2 = new ColidEntrySubscription() { ColidPidUri = subscription.ColidPidUri, User = user2 };

            _seeder.Add(subscription);
            _seeder.Add(subscription2);

            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent(new HashSet<Uri>() { subscription.ColidPidUri }));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ColidEntrySubscriptionAmountDto>>(stringResponse);

            Assert.Single(result);
            Assert.Equal(subscription.ColidPidUri, result.First().ColidPidUri);
            Assert.Equal(2, result.First().Subscriptions);
        }

        [Fact]
        public async Task GetAmountOfSubscribers_Post_TwoUsers_TwoSubscribedPidUris_GetOnePidUri_Returns_NumberOfSubscribers()
        {
            _seeder.ClearUsers();
            _seeder.SeedMessageTemplates();

            var user = TestData.GenerateRandomUser();
            var user2 = TestData.GenerateRandomUser();

            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            var subscription2 = TestData.GenerateRandomColidEntrySubscription();
            subscription2.User = user2;

            _seeder.Add(subscription);
            _seeder.Add(subscription2);

            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent(new HashSet<Uri>() { subscription.ColidPidUri }));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ColidEntrySubscriptionAmountDto>>(stringResponse);

            Assert.Single(result);
            Assert.Equal(subscription.ColidPidUri, result.First().ColidPidUri);
            Assert.Equal(1, result.First().Subscriptions);
        }

        [Fact]
        public async Task GetAmountOfSubscribers_Post_TwoUsers_TwoSubscribedPidUris_GetTwoPidUri_Returns_NumberOfSubscribers()
        {
            _seeder.ClearUsers();
            _seeder.SeedMessageTemplates();

            var user = TestData.GenerateRandomUser();
            var user2 = TestData.GenerateRandomUser();

            var subscription = TestData.GenerateRandomColidEntrySubscription();
            subscription.User = user;
            var subscription2 = TestData.GenerateRandomColidEntrySubscription();
            subscription2.User = user2;

            _seeder.Add(subscription);
            _seeder.Add(subscription2);

            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent(new HashSet<Uri>() { subscription.ColidPidUri, subscription2.ColidPidUri }));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ColidEntrySubscriptionAmountDto>>(stringResponse);

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.FirstOrDefault(s => s.ColidPidUri == subscription.ColidPidUri && s.Subscriptions == 1));
            Assert.NotNull(result.FirstOrDefault(s => s.ColidPidUri == subscription2.ColidPidUri && s.Subscriptions == 1));
        }

        [Fact]
        public async Task GetAmountOfSubscribers_Post_EmptyList_Returns_BadRequest()
        {
            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent(new HashSet<Uri>()));
            await response.Content.ReadAsStringAsync();

            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAmountOfSubscribers_Post_Null_Returns_BadRequest()
        {
            var response = await Client.PostAsync($"{PATH}/subscriptions", _api.BuildJsonHttpContent<HashSet<Uri>>(null));
            await response.Content.ReadAsStringAsync();

            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
        }
    }
}
