using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Constants;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace COLID.AppDataService.Services.Graph.Implementation
{
    public class RemoteMicrosoftGraphService : IRemoteGraphService
    {
        private readonly IGraphServiceClient _graphClient;

        private readonly ILogger<RemoteMicrosoftGraphService> _logger;

        public RemoteMicrosoftGraphService(IGraphServiceClient graphClient, ILogger<RemoteMicrosoftGraphService> logger)
        {
            _graphClient = graphClient;
            _logger = logger;
        }

        #region User

        public async Task<User> GetUserAsync(string id)
        {
            try
            {
                var user = await _graphClient.Users[id]
                                .Request()
                                .Select(u => new
                                {
                                    u.Id,
                                    u.GivenName,
                                    u.Surname,
                                    u.DisplayName,
                                    u.MailNickname,
                                    u.AccountEnabled,
                                    u.Mail
                                })
                                .GetAsync();

                return user;
            }
            catch (ServiceException e)
            {
                throw HandleServiceException(e, id);
            }
        }

        public async Task<string> GetManagerIdOfUserAsync(string id)
        {
            try
            {
                var user = await _graphClient.Users[id]
                                .Manager
                                .Request()
                                .Select(u => u.Id)
                                .GetAsync();

                return user.Id;
            }
            catch (ServiceException e)
            {
                throw HandleServiceException(e, id);
            }
        }

        public async Task<IList<User>> FindUserAsync(string query, string additionalBaseFilter)
        {
            try
            {
                var baseFilter = "onPremisesSyncEnabled eq true";
                if (!string.IsNullOrEmpty(additionalBaseFilter))
                {
                    baseFilter += $" and {additionalBaseFilter}";
                }

                var searchFieldFilter = $"startswith(mailNickname, '{query}')" +
                    $" or startswith(displayName, '{query}')" +
                    $" or startswith(surName, '{query}')" +
                    $" or startswith(givenName, '{query}')" +
                    $" or startswith(mail, '{query}')";

                var users = await _graphClient.Users
                                .Request()
                                .Filter($"{baseFilter} and ({searchFieldFilter})")
                                .Select(u => new
                                {
                                    u.Id,
                                    u.GivenName,
                                    u.Surname,
                                    u.DisplayName,
                                    u.MailNickname,
                                    u.AccountEnabled,
                                    u.Mail
                                })
                                .GetAsync();

                if (users == null || !users.Any())
                {
                    throw new EntityNotFoundException($"No users found for search term '{query}'", query);
                }

                return users;
            }
            catch (ServiceException e)
            {
                throw HandleSearchServiceException(e, query);
            }
        }

        public async Task<IList<User>> FindActiveUserAsync(string query)
        {
            var baseFilter = "accountEnabled eq true";
            var users = await FindUserAsync(query, baseFilter);

            if (users == null || !users.Any())
            {
                throw new EntityNotFoundException($"No users found for search term '{query}'", query);
            }

            return users;
        }

        public async Task<IList<AdUserDto>> CheckUsersValidityAsync(ISet<string> adUserEmailSet)
        {
            Guard.IsNotNullOrEmpty(adUserEmailSet);
            var adUserList = new List<AdUserDto>();

            foreach (var mail in adUserEmailSet)
            {
                var adUserDto = new AdUserDto(null, mail, false);
                try
                {
                    var userRequest = _graphClient.Users[mail]
                        .Request()
                        .Select(u => new { u.Id, u.Mail, u.AccountEnabled });

                    var adUser = await userRequest.GetAsync();
                    if (adUser != null)
                    {
                        adUserDto.Id = adUser.Id;
                        adUserDto.AccountEnabled = (true == adUser.AccountEnabled);
                    }
                }
                catch (ServiceException e)
                {
                    if (!e.Message.Contains("Request_ResourceNotFound"))
                    {
                        throw e;
                    }
                }
                finally
                {
                    adUserList.Add(adUserDto);
                }
            }

            return adUserList;
        }

        #endregion User

        #region Group

        public async Task<Group> GetGroupAsync(string id)
        {
            try
            {
                var baseFilter = "onPremisesSyncEnabled eq true and mailEnabled eq true";
                var additionalFilter = $"mail eq '{id}'";

                var groups = await _graphClient.Groups
                                .Request()
                                .Filter($"{baseFilter} and ({additionalFilter})")
                                .Select(u => new
                                {
                                    u.Id,
                                    u.DisplayName,
                                    u.MailEnabled,
                                    u.Description,
                                    u.Mail
                                })
                                .GetAsync();

                if (groups == null || !groups.Any())
                {
                    throw new EntityNotFoundException($"Resource '{id}' was not found.", id);
                }

                return groups.FirstOrDefault();
            }
            catch (ServiceException e)
            {
                throw HandleServiceException(e, id);
            }
        }

        public async Task<IList<Group>> FindGroupAsync(string query)
        {
            try
            {
                var baseFilter = "onPremisesSyncEnabled eq true and mailEnabled eq true";
                var additionalFilter = $"startswith(mail, '{query}') or startswith(displayName, '{query}')";

                var groups = await _graphClient.Groups
                                .Request()
                                .Filter($"{baseFilter} and ({additionalFilter})")
                                .Select(u => new
                                {
                                    u.Id,
                                    u.DisplayName,
                                    u.MailEnabled,
                                    u.Description,
                                    u.Mail
                                })
                                .GetAsync();

                if (groups == null || !groups.Any())
                {
                    throw new EntityNotFoundException($"No groups found for search term '{query}'", query);
                }

                return groups;
            }
            catch (ServiceException e)
            {
                throw HandleSearchServiceException(e, query);
            }
        }

        #endregion Group

        #region Handlers

        private System.Exception HandleSearchServiceException(ServiceException serviceException, string query)
        {
            switch (serviceException.Error.Code)
            {
                case MicrosoftConstants.Exception.Codes.BadRequest:
                    throw new QueryException($"The given search term '{query}' is invalid.", serviceException.InnerException);
                default:
                    throw new System.Exception($"An unknown error has occurred while searching for users.", serviceException.InnerException);
            }
        }

        private System.Exception HandleServiceException(ServiceException serviceException, string id)
        {
            switch (serviceException.Error.Code)
            {
                case MicrosoftConstants.Exception.Codes.RequestResourceNotFound:
                case MicrosoftConstants.Exception.Codes.ResourceNotFound:
                case MicrosoftConstants.Exception.Codes.ErrorItemNotFound:
                case MicrosoftConstants.Exception.Codes.ItemNotFound:
                    return new EntityNotFoundException($"Resource '{id}' was not found.", id);

                case MicrosoftConstants.Exception.Codes.BadRequest:
                    return new QueryException($"The requested resource '{id}' is invalid.");

                default:
                    return new System.Exception($"An unknown error has occurred while fetching resource with id: {id}.");
            }
        }

        #endregion Handlers
    }
}
