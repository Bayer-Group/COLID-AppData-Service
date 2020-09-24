using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace COLID.AppDataService.Common.Utilities
{
    public class GraphAuthProvider : IAuthenticationProvider
    {
        private readonly IConfidentialClientApplication _msalClient;
        private readonly int _maxRetries = 3;

        public GraphAuthProvider(IOptionsMonitor<AzureADOptions> azureAdOptions)
        {
            var currentAzureAdOptions = azureAdOptions.CurrentValue;

            _msalClient = ConfidentialClientApplicationBuilder
                .Create(currentAzureAdOptions.ClientId)
                .WithTenantId(currentAzureAdOptions.TenantId)
                .WithClientSecret(currentAzureAdOptions.ClientSecret)
                .Build();
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            int retryCount = 0;

            do
            {
                try
                {
                    var result = await _msalClient
                        .AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                        .ExecuteAsync();

                    if (!string.IsNullOrEmpty(result.AccessToken))
                    {
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("bearer", result.AccessToken);
                        break;
                    }
                }
                catch (MsalServiceException serviceException)
                {
                    if (serviceException.ErrorCode == "temporarily_unavailable")
                    {
                        var delay = GetRetryAfter(serviceException);
                        await Task.Delay(delay);
                    }
                    else
                    {
                        throw new AuthenticationException("Unexpected exception returned from MSAL.", serviceException);
                    }
                }
                catch (System.Exception exception)
                {
                    throw new AuthenticationException("Unexpected exception occurred while authenticating the request",
                        exception);
                }

                retryCount++;
            } while (retryCount < _maxRetries);
        }

        private TimeSpan GetRetryAfter(MsalServiceException serviceException)
        {
            var retryAfter = serviceException.Headers?.RetryAfter;
            TimeSpan? delay = null;

            if (retryAfter != null && retryAfter.Delta.HasValue)
            {
                delay = retryAfter.Delta;
            }
            else if (retryAfter != null && retryAfter.Date.HasValue)
            {
                delay = retryAfter.Date.Value.Offset;
            }

            if (delay == null)
            {
                throw new MsalServiceException(
                    serviceException.ErrorCode,
                    "Missing Retry-After header."
                );
            }

            return delay.Value;
        }
    }
}
