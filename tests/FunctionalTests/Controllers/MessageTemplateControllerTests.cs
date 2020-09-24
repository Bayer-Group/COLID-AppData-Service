using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
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
    public class MessageTemplateControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/messageTemplates";

        private readonly TestDataContextSeeder _seeder;
        private readonly ApiTestHelper _api;
        private readonly ITestOutputHelper _output;

        private readonly MessageTemplate _tplColidEntrySubscription;
        private readonly MessageTemplate _tplStoredQueryResult;

        public MessageTemplateControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;

            _tplColidEntrySubscription = TestData.GetPreconfiguredMessageTemplates().Where(e => e.Type.Equals(MessageType.ColidEntrySubscriptionUpdate)).First();
            _tplStoredQueryResult = TestData.GetPreconfiguredMessageTemplates().Where(e => e.Type.Equals(MessageType.StoredQueryResult)).First();
        }

        [Fact]
        public async Task Get_All_Returns_Ok()
        {
            int expectedAmount = TestData.GetPreconfiguredMessageTemplates().Count();

            _seeder.ResetMessageTemplates();
            var response = await Client.GetAsync(PATH);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseTemplates = JsonConvert.DeserializeObject<List<MessageTemplate>>(stringResponse);

            Assert.NotNull(responseTemplates);
            Assert.Equal(expectedAmount, responseTemplates.Count);

            var cesTpl = responseTemplates.Where(t => MessageType.ColidEntrySubscriptionUpdate.Equals(t.Type)).FirstOrDefault();
            Assert.Equal(MessageType.ColidEntrySubscriptionUpdate, cesTpl.Type);
            Assert.Equal(_tplColidEntrySubscription.Body, cesTpl.Body);
            Assert.Equal(_tplColidEntrySubscription.Subject, cesTpl.Subject);
        }

        [Fact]
        public async Task Get_One_Returns_Ok()
        {
            _seeder.ResetMessageTemplates();
            var messageType = MessageType.StoredQueryResult;
            var response = await Client.GetAsync($"{PATH}/{messageType}"); // will be converted into string
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseTemplate = JsonConvert.DeserializeObject<MessageTemplate>(stringResponse);

            Assert.NotNull(responseTemplate);

            Assert.Equal(MessageType.StoredQueryResult, responseTemplate.Type);
            Assert.Equal(_tplStoredQueryResult.Body, responseTemplate.Body);
            Assert.Equal(_tplStoredQueryResult.Subject, responseTemplate.Subject);
        }

        [Fact]
        public async Task Get_One_ByInt_Returns_Ok()
        {
            var msgTpl = TestData.GetPreconfiguredMessageTemplates()
                .FirstOrDefault(x => MessageType.StoredQueryResult.Equals(x.Type));

            _seeder.ResetMessageTemplates();
            var response = await Client.GetAsync($"{PATH}/{(int) MessageType.StoredQueryResult}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseTemplate = JsonConvert.DeserializeObject<MessageTemplate>(stringResponse);

            Assert.NotNull(responseTemplate);

            Assert.Equal(MessageType.StoredQueryResult, responseTemplate.Type);
            Assert.Equal(_tplStoredQueryResult.Body, responseTemplate.Body);
            Assert.Equal(_tplStoredQueryResult.Subject, responseTemplate.Subject);
        }

        [Fact]
        public async Task Get_One_Returns_BadRequest_IfTypeDoesNotExist()
        {
            await _api.Get_ShouldReturn_BadRequest_IfIdHasTypeMismatch($"{PATH}/NonExistingType");
        }

        [Fact]
        public async Task Post_Returns_OK()
        {
            _seeder.ClearMessageTemplates(); // so no templates are in DB
            var tpl = TestData.GenerateSampleMessageTemplateForColidEntrySubscriptionUpdateDto();

            var response = await Client.PostAsync(PATH, _api.BuildJsonHttpContent(tpl));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            response.EnsureSuccessStatusCode();

            var responseTemplate = JsonConvert.DeserializeObject<MessageTemplate>(stringResponse);
            Assert.NotNull(responseTemplate);
            Assert.NotEqual(0, responseTemplate.Id);
            Assert.Equal(tpl.Type, responseTemplate.Type);
            Assert.Equal(tpl.Subject, responseTemplate.Subject);
            Assert.Equal(tpl.Body, responseTemplate.Body);

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public async Task Post_Returns_BadRequest_IfTemplateAlreadyExists()
        {
            _seeder.ResetMessageTemplates();

            var unchangedTpl = new MessageTemplateBuilder()
                .WithType(_tplColidEntrySubscription.Type)
                .WithBody(_tplColidEntrySubscription.Body)
                .WithSubject(_tplColidEntrySubscription.Subject)
                .BuildDto();

            await _api.Post_ShouldReturn_BadRequest_IfEntityAlreadyExists(PATH, _api.BuildJsonHttpContent(unchangedTpl));
        }

        [Fact]
        public async Task Post_Returns_BadRequest_IfContentIsEmpty()
        {
            await _api.Post_ShouldReturn_BadRequest_IfJsonContentIsEmpty(PATH);
        }

        [Fact]
        public async Task Post_Returns_BadRequest_IfContentIsNull()
        {
            await _api.Post_ShouldReturn_BadRequest_IfContentIsNull(PATH);
        }

        [Fact]
        public async Task Update_Returns_Ok()
        {
            _seeder.ResetMessageTemplates();

            var original = _tplColidEntrySubscription;
            _output.WriteLine($"before: {original}");
            var updatedSubject = "This is a new subject";
            var updatedBody = "Super special new body 5000";
            var updatedDto = new MessageTemplateBuilder()
                .WithType(MessageType.ColidEntrySubscriptionUpdate)
                .WithBody(updatedBody)
                .WithSubject(updatedSubject)
                .BuildDto();

            var response = await Client.PutAsync($"{PATH}", _api.BuildJsonHttpContent(updatedDto));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"after: {stringResponse}");
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<MessageTemplate>(stringResponse);

            Assert.NotNull(responseMessage);
            Assert.NotEqual(original.Body, responseMessage.Body);
            Assert.NotEqual(original.Subject, responseMessage.Subject);
            Assert.Equal(updatedDto.Body, responseMessage.Body);
            Assert.Equal(updatedDto.Subject, responseMessage.Subject);

            _seeder.ResetMessageTemplates();
        }

        [Fact]
        public async Task Update_Returns_Ok_IfTemplateHasntChanged()
        {
            _seeder.ResetMessageTemplates();

            var unchangedTpl = new MessageTemplateBuilder()
                .WithType(_tplColidEntrySubscription.Type)
                .WithBody(_tplColidEntrySubscription.Body)
                .WithSubject(_tplColidEntrySubscription.Subject)
                .BuildDto();

            var response = await Client.PutAsync($"{PATH}", _api.BuildJsonHttpContent(unchangedTpl));
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);
        }
    }
}
