using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.AppDataService.Services.Interfaces;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.AppDataService.Common.Search;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Services.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace COLID.AppDataService.Services.Implementation
{
    internal class RemoteSearchService : IRemoteSearchService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITokenService<SearchServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteSearchService> _logger;
        private readonly bool _bypassProxy;
        private readonly string _userEndpoint;
        private readonly string _searchServiceDocumentApi;

        public RemoteSearchService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContext,
            ITokenService<SearchServiceTokenOptions> tokenService,
            ILogger<RemoteSearchService> logger
            )
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContext = correlationContext;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;
            _bypassProxy = _configuration.GetValue<bool>("BypassProxy");
            var serverUrl = _configuration.GetConnectionString("SearchServiceUrl");
            _userEndpoint = $"{serverUrl}/api/Search";
            _searchServiceDocumentApi = $"{serverUrl}/api/documentsByIds";

        }

        public async Task<SearchResultDTO> Search(SearchRequestDto searchRequest)
        {
            using (var httpClient = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
            {
                HttpResponseMessage response = null;
                SearchResultDTO responseContent = null;
                try
                {
                    var path = $"{_userEndpoint}";
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                        searchRequest, accessToken, _cancellationToken);
                    response.EnsureSuccessStatusCode();
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsAsync<SearchResultDTO>();
                    }
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError("An error occured", ex);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Couldn't connect to the remote search service.", ex);
                }

                return responseContent;
            }
        }

        public async Task<IDictionary<string, IEnumerable<JObject>>> GetDocumentsByIds(IEnumerable<string> identifiers)
        {
            using (var httpClient = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
            {
                try
                {
                    // Encode the searchRequest into a JSON object for sending
                    string jsonobject = JsonConvert.SerializeObject(identifiers);
                    using StringContent content = new StringContent(jsonobject, Encoding.UTF8, "application/json");

                    //Fetch token for search service
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    // Post the JSON object to the SearchService endpoint
                    HttpResponseMessage response = await httpClient.PostAsync(_searchServiceDocumentApi, content);
                    response.EnsureSuccessStatusCode();
                    var result = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<JObject>>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
                    return result;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("An error occurred while passing the search request to the search service GetDocumentsByIds.", ex);
                    throw ex;
                }
            }
        }
    }
}
