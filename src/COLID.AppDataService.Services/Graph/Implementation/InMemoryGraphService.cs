using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Services.Graph.Interface;
using Microsoft.Graph;

namespace COLID.AppDataService.Services.Graph.Implementation
{
    public class InMemoryGraphService : IRemoteGraphService
    {
        public InMemoryGraphService()
        {
        }

        public Task<IList<AdUserDto>> CheckUsersValidityAsync(ISet<string> adUserEmailSet)
        {
            IList<AdUserDto> userList = new List<AdUserDto>();
            foreach (var email in adUserEmailSet)
            {
                userList.Add(new AdUserDto(Guid.NewGuid().ToString(), email, true));
            }
            return Task.FromResult(userList);
        }

        public Task<Group> GetGroupAsync(string id)
        {
            var group = GetGroups().FirstOrDefault();
            group.Id = id;
            return Task.FromResult(group);
        }

        public Task<string> GetManagerIdOfUserAsync(string id)
        {
            var user = GetUsers().FirstOrDefault();
            return Task.FromResult(id);
        }

        public Task<User> GetUserAsync(string id)
        {
            var user = GetUsers().FirstOrDefault();
            user.Id = id;
            return Task.FromResult(user);
        }

        public Task<IList<Group>> FindGroupAsync(string query)
        {
            var groups = GetGroups();
            return Task.FromResult(groups);
        }

        public Task<IList<User>> FindActiveUserAsync(string query)
        {
            var users = GetUsers();
            return Task.FromResult(users);
        }

        private IList<Group> GetGroups()
        {
            return new List<Group>
            {
                BuildGroup("Development Group 1", "colid.group.1@bayer.com"),
                BuildGroup("Development Group 2", "colid.group.2@bayer.com"),
                BuildGroup("Development Group 3", "colid.group.3@bayer.com"),
                BuildGroup("Development Group 4", "colid.group.4@bayer.com"),
                BuildGroup("Development Group 5", "colid.group.5@bayer.com")
            };
        }

        private Group BuildGroup(string displayName, string mail, bool mailEnabled = true)
        {
            return new Group()
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = displayName,
                MailEnabled = mailEnabled,
                Description = "Some random group for development purpose",
                Mail = mail
            };
        }

        private IList<User> GetUsers()
        {
            return new List<User>
            {
                BuildUser("Jenson", "Johnson", "EHDTE"),
                BuildUser("Peter", "Russell", "HEUDN"),
                BuildUser("Harris", "Taylor", "OWEND"),
                BuildUser("Megan", "Wilson", "LFHEN"),
                BuildUser("Marcus", "Davies", "KNBWI"),
                BuildUser("Super", "Admin", "HEODW", "superadmin@bayer.com"),
                BuildUser("Another", "User", "HEUSW", "anotheruser@bayer.com")
            };
        }

        private User BuildUser(string givenName, string surname, string mailNickname, string mail = null, bool accountEnabled = true)
        {
            return new User()
            {
                Id = Guid.NewGuid().ToString(),
                GivenName = givenName,
                Surname = surname,
                DisplayName = $"{givenName} {surname}",
                MailNickname = mailNickname,
                AccountEnabled = accountEnabled,
                Mail = mail ?? $"{givenName}.{surname}.{mailNickname}@bayer.com"
            };
        }
    }
}
