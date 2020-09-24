using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class ConsumerGroupControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/consumerGroups";

        private readonly TestDataContextSeeder _seeder;

        private readonly ITestOutputHelper _output;

        private readonly ApiTestHelper _api;

        public ConsumerGroupControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;
        }

        [Fact]
        public async Task Get_All_Returns_Ok()
        {
            _seeder.ResetConsumerGroups(); // Reset to initial, so delete and create doesn't change the value
            var response = await Client.GetAsync(PATH);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var cgs = JsonConvert.DeserializeObject<List<ConsumerGroup>>(stringResponse);

            Assert.Equal(4, cgs.Count());
        }

        [Fact]
        public async Task Post_Returns_OK()
        {
            var cg = TestData.GenerateRandomConsumerGroupDto();

            var response = await Client.PostAsync(PATH, _api.BuildJsonHttpContent(cg));
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            response.EnsureSuccessStatusCode();

            var responseCg = JsonConvert.DeserializeObject<ConsumerGroup>(stringResponse);
            Assert.NotNull(responseCg);
            Assert.NotEqual(0, responseCg.Id);
            Assert.Equal(cg.Uri, responseCg.Uri);
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
        public async Task Delete_Returns_Ok()
        {
            var cg = _seeder.Add(TestData.GenerateRandomConsumerGroup());
            var response = await _api.SendDeleteRequestWithContent(PATH, _api.BuildJsonHttpContent(cg));
            await _api.PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.OK);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_Returns_BadRequest_IfContentIsNull()
        {
            await _api.DeleteWithContent_ShouldReturn_BadRequest_IfContentIsNull(PATH);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_IfConsumerGroupDoesNotExist()
        {
            var nonExistingCg = new ConsumerGroupBuilder().WithUri(new Uri("http://incredibleUri2020/")).BuildDto();
            await _api.DeleteWithContent_ShouldReturn_NotFound_IfEntityDoesNotExist(PATH, _api.BuildJsonHttpContent(nonExistingCg));
        }
    }
}
