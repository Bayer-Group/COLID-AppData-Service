using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.Cache.Services;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Graph.Implementation
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IRemoteGraphService _remoteGraphService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly ILogger<ActiveDirectoryService> _logger;

        public ActiveDirectoryService(IRemoteGraphService remoteGraphService, IMapper mapper, ICacheService cache, ILogger<ActiveDirectoryService> logger)
        {
            _remoteGraphService = remoteGraphService;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        #region User

        public async Task<AdUser> GetUserAsync(string id)
        {
            Guard.IsValidGuidOrEmail(id);

            var user = await _remoteGraphService.GetUserAsync(id);

            if (IsActiveUser(user))
            {
                var adUser = _mapper.Map<AdUser>(user);
                return adUser;
            }

            throw new EntityNotFoundException($"User '{id}' was not found.", id);
        }

        public async Task<AdUser> GetManagerByUserIdAsync(string id)
        {
            Guard.IsValidGuidOrEmail(id);

            var managerId = await _remoteGraphService.GetManagerIdOfUserAsync(id);
            var manager = await GetUserAsync(managerId);

            return manager;
        }

        public async Task<IList<AdUser>> FindUsersAsync(string query)
        {
            Guard.IsNotEmpty(query);

            var users = await _remoteGraphService.FindActiveUserAsync(query);

            var adUsers = users
                .Select(user => _mapper.Map<AdUser>(user))
                .ToList();

            return adUsers;
        }

        #endregion User

        #region Group

        public async Task<AdGroup> GetGroupAsync(string id)
        {
            Guard.IsValidEmail(id);

            var group = await _remoteGraphService.GetGroupAsync(id);

            if (IsActiveGroup(group))
            {
                var adGroup = _mapper.Map<AdGroup>(group);
                return adGroup;
            }

            throw new EntityNotFoundException($"Group '{id}' was not found.", id);
        }

        public async Task<IList<AdGroup>> FindGroupsAsync(string query)
        {
            Guard.IsNotEmpty(query);

            var groups = await _remoteGraphService.FindGroupAsync(query);

            var adGroups = groups
                .Select(group => _mapper.Map<AdGroup>(group))
                .ToList();

            return adGroups;
        }

        #endregion Group

        public AdSearchResult FindUsersAndGroupsAsync(string query)
        {
            Guard.IsNotEmpty(query);

            var userSearchTask = ProcessGetAction(FindUsersAsync, query);
            var groupSearchTask = ProcessGetAction(FindGroupsAsync, query);

            Task.WaitAll(userSearchTask, groupSearchTask);

            var users = userSearchTask.Result;
            var groups = groupSearchTask.Result;

            if (!users.Any() && !groups.Any())
            {
                throw new EntityNotFoundException($"No groups and users were found for search term '{query}'", query);
            }

            return new AdSearchResult(groups, users);
        }

        public AdObject GetUserOrGroupAsync(string id)
        {
            Guard.IsValidGuidOrEmail(id);

            var userTask = ProcessGetAction(GetUserAsync, id);
            var groupTask = ProcessGetAction(GetGroupAsync, id);

            Task.WaitAll(userTask, groupTask);

            var user = userTask.Result;
            var group = groupTask.Result;

            if (user != null)
            {
                return user;
            }

            if (group != null)
            {
                return group;
            }

            throw new EntityNotFoundException($"No group or user was found for given id {id}", id);
        }

        public async Task<IEnumerable<AdUserDto>> CheckUsersValidityAsync(ISet<string> adUserEmailSet)
        {
            Guard.ContainsValidEmails(adUserEmailSet);

            var adUsersToCheck = new HashSet<string>();
            var checkedUsers = new List<AdUserDto>();

            foreach (var userEmail in adUserEmailSet)
            {
                // if invalid ones exists, put them in a separate list (in case that only invalid are asked)
                if (_cache.Exists(userEmail))
                {
                    var invalidUserFromCache = new AdUserDto(null, userEmail, false);
                    checkedUsers.Add(invalidUserFromCache);
                    continue;
                }
                adUsersToCheck.Add(userEmail);
            }

            if (adUsersToCheck.Any())
            {
                var usersFromGraph = await _remoteGraphService.CheckUsersValidityAsync(adUsersToCheck);
                WriteInvalidUsersToCache(usersFromGraph);
                checkedUsers.AddRange(usersFromGraph);
            }

            return checkedUsers;
        }

        private void WriteInvalidUsersToCache(IList<AdUserDto> adUserDtoList)
        {
            var filteredList = adUserDtoList
                .Where(user => user.AccountEnabled == false)
                .Select(user => user.Mail)
                .ToList();
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            filteredList.ForEach(invalidUser => _cache.Set(invalidUser, now));
        }

        private async Task<IList<T>> ProcessGetAction<T>(Func<string, Task<IList<T>>> action, string query)
        {
            IList<T> results;
            try
            {
                results = await action.Invoke(query);
            }
            catch (System.Exception ex) when (ex is EntityNotFoundException || ex is FormatException || ex is ArgumentException)
            {
                results = new List<T>();
            }

            return results;
        }

        private async Task<T> ProcessGetAction<T>(Func<string, Task<T>> action, string query)
        {
            try
            {
                return await action.Invoke(query);
            }
            catch (System.Exception ex) when (ex is EntityNotFoundException || ex is FormatException || ex is ArgumentException)
            {
                return default;
            }
        }

        private bool IsActiveGroup(Microsoft.Graph.Group group)
        {
            return group.MailEnabled ?? false;
        }

        private bool IsActiveUser(Microsoft.Graph.User user)
        {
            return user.AccountEnabled ?? false;
        }
    }
}
