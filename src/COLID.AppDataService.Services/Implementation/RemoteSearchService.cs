using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COLID.AppDataService.Services.Interface;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.AppDataService.Common.Search;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Services.Configuration;

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

        private readonly string _userEndpoint;

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

            var serverUrl = _configuration.GetConnectionString("SearchServiceUrl");
            _userEndpoint = $"{serverUrl}/api/Search";
        }

        public async Task<SearchResultDTO> Search(SearchRequestDto searchRequest)
        {
            using var httpClient = _clientFactory.CreateClient();
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
}
