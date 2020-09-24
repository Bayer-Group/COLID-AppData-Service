using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Tests.Unit.Builder;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Functional
{
    public class ApiTestHelper
    {
        private HttpClient Client { get; }

        private readonly ITestOutputHelper _output;

        private const string PATH_USERS = "/api/users";

        public ApiTestHelper(HttpClient client, ITestOutputHelper output)
        {
            Client = client;
            _output = output;
        }

        #region WORK-IN-PROGRESS

        public async Task CreateUser(User user)
        {
            HttpContent requestContent = new StringContent(user.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await Client.PostAsync(PATH_USERS, requestContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("CreateUser: " + stringResponse + "\n");
            response.EnsureSuccessStatusCode();
            var responseUser = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.NotNull(responseUser);
            Assert.Equal(user.Id, responseUser.Id);
            Assert.Equal(user.EmailAddress, responseUser.EmailAddress);
        }

        public async Task CreateUser(Guid userId)
        {
            var email = "john.finlay@tiger.king";
            var user = new UserBuilder().WithId(userId).WithEmailAddress(email).Build();
            await CreateUser(user);
        }

        public async Task<User> GetUser(Guid userId)
        {
            var response = await Client.GetAsync($"{PATH_USERS}/{userId}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Found User: " + stringResponse + "\n");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            response.EnsureSuccessStatusCode();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);
            Assert.NotNull(user);

            return user;
        }

        #endregion WORK-IN-PROGRESS

        #region GET

        public async Task<HttpResponseMessage> Get_ShouldReturn_NotFound_IfEntityDoesNotExist(string path)
        {
            var response = await Client.GetAsync(path);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.NotFound);
            return response;
        }

        public async Task<HttpResponseMessage> Get_ShouldReturn_BadRequest_IfIdHasTypeMismatch(string path)
        {
            var response = await Client.GetAsync(path);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        #endregion GET

        #region POST

        public async Task<HttpResponseMessage> Post_ShouldReturn_BadRequest_IfEntityAlreadyExists(string path, HttpContent content)
        {
            var response = await Client.PostAsync(path, BuildJsonHttpContent(content));
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Post_ShouldReturn_BadRequest_IfTextContentIsEmpty(string path)
        {
            var response = await Client.PostAsync(path, BuildPlainTextHttpContent(""));
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Post_ShouldReturn_BadRequest_IfJsonContentIsEmpty(string path)
        {
            var response = await Client.PostAsync(path, BuildJsonHttpContent(""));
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Post_ShouldReturn_BadRequest_IfContentIsNull(string path)
        {
            var response = await Client.PostAsync(path, null);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        #endregion POST

        #region PUT

        public async Task<HttpResponseMessage> Put_ShouldReturn_NotFound_IfEntityDoesNotExist(string path, HttpContent content)
        {
            var response = await Client.PutAsync(path, content);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.NotFound);
            return response;
        }

        public async Task<HttpResponseMessage> Put_ShouldReturn_BadRequest_IfIdHasTypeMismatch(string path, HttpContent content)
        {
            var response = await Client.PutAsync(path, content);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Put_ShouldReturn_BadRequest_IfTextContentIsEmpty(string path)
        {
            var response = await Client.PutAsync(path, BuildPlainTextHttpContent(""));
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Put_ShouldReturn_BadRequest_IfJsonContentIsEmpty(string path)
        {
            var response = await Client.PutAsync(path, BuildJsonHttpContent(""));
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> Put_ShouldReturn_BadRequest_IfContentIsNull(string path)
        {
            var response = await Client.PutAsync(path, null);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        #endregion PUT

        #region DELETE

        public async Task<HttpResponseMessage> Delete_ShouldReturn_NotFound_IfEntityDoesNotExist(string path)
        {
            var response = await Client.DeleteAsync(path);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.NotFound);
            return response;
        }

        public async Task<HttpResponseMessage> Delete_ShouldReturn_BadRequest_IfIdHasTypeMismatch(string path)
        {
            // e.g. when you expect a GUID within a path and pass an Integer or String.
            var response = await Client.DeleteAsync(path);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteWithContent_ShouldReturn_BadRequest_IfContentIsNull(string path)
        {
            var response = await SendDeleteRequestWithContent(path, null);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteWithContent_ShouldReturn_NotFound_IfEntityDoesNotExist(string path, HttpContent content)
        {
            var response = await SendDeleteRequestWithContent(path, content);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.NotFound);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteWithContent_ShouldReturn_BadRequest_IfIdHasTypeMismatch(string path, HttpContent content)
        {
            var response = await SendDeleteRequestWithContent(path, content);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteWithContent_ShouldReturn_BadRequest_IfContentHasTypeMismatch(string path, HttpContent content)
        {
            var response = await SendDeleteRequestWithContent(path, content);
            await PrintResponseContentAndAssertStatusCode(response, HttpStatusCode.BadRequest);
            return response;
        }

        #endregion DELETE

        // REQUEST FUNCTIONS

        public async Task<HttpResponseMessage> SendDeleteRequestWithContent(string path, HttpContent requestContent)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, path)
            {
                Content = requestContent
            };
            return await Client.SendAsync(request);
        }

        // OTHER HELPER FUNCTIONS

        public async Task PrintResponseContentAndAssertStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            _output.WriteLine(stringResponse);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        public HttpContent BuildJsonHttpContent<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            HttpContent requestContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            return requestContent;
        }

        public HttpContent BuildPlainTextHttpContent<T>(T obj)
        {
            return new StringContent(obj.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain);
        }

        public async Task<T> DeserializeHttpContent<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}
