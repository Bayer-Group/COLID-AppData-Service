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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Configuration;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Services.Implementation
{
    internal class RemoteRegistrationService : IRemoteRegistrationService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenService<RegistrationServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteRegistrationService> _logger;
        private readonly bool _bypassProxy;
        private readonly string _registrationServiceSaveSearchApi;
        private readonly string _registrationServiceRemoveNginxConfigApi;

        public RemoteRegistrationService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITokenService<RegistrationServiceTokenOptions> tokenService,
            ILogger<RemoteRegistrationService> logger
            )
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;
            _bypassProxy = _configuration.GetValue<bool>("BypassProxy");
            var serverUrl = _configuration.GetConnectionString("RegistrationServiceUrl");
            _registrationServiceSaveSearchApi = $"{serverUrl}/api/v3/identifier/savedsearch";
            _registrationServiceRemoveNginxConfigApi = $"{serverUrl}/api/v3/proxyConfig/removeSearchFilterProxy";


        }

        public async Task<SearchFilterDataMarketplaceDto> RegisterSavedSearches(SearchFilterDataMarketplaceDto searchFilterDataMarketplaceDto)
        {
            using (var httpClient = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
            {
                HttpResponseMessage response = null;
                SearchFilterDataMarketplaceDto responseContent = null;
                try
                {
                    var path = $"{_registrationServiceSaveSearchApi}";
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Post, path,
                        searchFilterDataMarketplaceDto, accessToken, _cancellationToken);
                    response.EnsureSuccessStatusCode();
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsAsync<SearchFilterDataMarketplaceDto>();
                    }
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError("An Authentication error occured to connect to the remote registration service", ex);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Couldn't connect to the remote registration service.", ex);
                }

                return responseContent;
            }
        }

        public async Task RemovePidUriFromConfig(string pidUri)
        {
            using (var httpClient = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
            {
                HttpResponseMessage response = null;
                try
                {
                    var path = $"{_registrationServiceRemoveNginxConfigApi}";
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    response = await httpClient.SendRequestWithBearerTokenAsync(HttpMethod.Delete, path,
                        pidUri, accessToken, _cancellationToken);
                    response.EnsureSuccessStatusCode();
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError("An Authentication error occured to connect to the remote registration service", ex);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Couldn't connect to the remote registration service.", ex);
                }
            }
        }
    }
}
