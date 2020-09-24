using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

using System.Collections.Generic;

using COLID.AppDataService.Tests.Integration;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;

using System;

using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional.Controllers
{
    [Collection("Sequential")]
    public class SchedulerControllerTests : IClassFixture<FunctionTestsFixture>
    {
        public HttpClient Client { get; }

        private const string PATH = "/api/scheduler";

        private readonly TestDataContextSeeder _seeder;
        private readonly ApiTestHelper _api;
        private readonly ITestOutputHelper _output;

        public SchedulerControllerTests(FunctionTestsFixture factory, ITestOutputHelper output)
        {
            Client = factory.CreateClient();
            var dbOptions = factory.DbContextOptions;
            _seeder = new TestDataContextSeeder(dbOptions);
            _api = new ApiTestHelper(factory.CreateClient(), output);
            _output = output;
        }

        [Fact]
        public async Task GetAllMessagesReadyToSend_Get_Returns_Ok()
        {
            // ARRANGE
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var subscription = _seeder.Add(TestData.GenerateRandomColidEntrySubscription());
            var message = _seeder.Add(new MessageBuilder().WithSubject("New COLID-Entry found for resource SPECIALMEH")
                .WithBody("A new entry with the glorious name SPECIALMEH in COLID has been found. Please take a look at http://meh")
                .WithSendOn(DateTime.Now.AddDays(-1)).Build());

            user.ColidEntrySubscriptions = new Collection<ColidEntrySubscription>() { subscription };
            user.Messages = new Collection<Message>() { message };
            var users = _seeder.Update(user);

            // ACT
            var response = await Client.GetAsync($"{PATH}/messages/toSend");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            response.EnsureSuccessStatusCode();
            var responseMessage = JsonConvert.DeserializeObject<IList<MessageUserDto>>(stringResponse);

            // ASSERT
            Assert.NotNull(responseMessage);
            var actualMsg = responseMessage.First();
            Assert.Equal(message.Subject, actualMsg.Subject);
            Assert.Equal(message.Body, actualMsg.Body);
            Assert.Null(actualMsg.ReadOn);
            Assert.True(message.SendOn < DateTime.Now);
            Assert.Null(actualMsg.DeleteOn);
            Assert.Equal(user.Id, actualMsg.UserId);
            Assert.Equal(user.EmailAddress, actualMsg.UserEmail);
        }
    }
}
