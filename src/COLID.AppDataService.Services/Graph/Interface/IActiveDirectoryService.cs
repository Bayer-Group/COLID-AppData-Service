using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Services.Graph.Interface
{
    public interface IActiveDirectoryService
    {
        /// <summary>
        /// Find one user by a given id and returns it.
        /// </summary>
        /// <param name="id">The user id to search for. The id can be the object id or email address.</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        Task<AdUser> GetUserAsync(string id);

        /// <summary>
        /// Find users by a given search term and returns them.
        /// </summary>
        /// <param name="query">The term to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given search term</exception>
        Task<IList<AdUser>> FindUsersAsync(string query);

        /// <summary>
        /// Find the manager of a user by a given id and returns it.
        /// </summary>
        /// <param name="id">The user id to search for. The id can be the object id (guid) or email address.</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        Task<AdUser> GetManagerByUserIdAsync(string id);

        /// <summary>
        /// Find one group by a given id and returns it.
        /// </summary>
        /// <param name="id">The group object id to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        /// <returns><see cref="AdGroup"/></returns>
        Task<AdGroup> GetGroupAsync(string id);

        /// <summary>
        /// Find groups by a given search term and returns them.
        /// </summary>
        /// <param name="query">The term to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no group was found for the given search term</exception>
        Task<IList<AdGroup>> FindGroupsAsync(string query);

        /// <summary>
        /// Find users and groups by a given search term and returns them.
        /// </summary>
        /// <param name="query">The term to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no groups and users was found for the given search term</exception>
        /// <returns><see cref="AdSearchResult"/></returns>
        AdSearchResult FindUsersAndGroupsAsync(string query);

        /// <summary>
        /// Find a user or group by a given id and returns them.
        /// </summary>
        /// <param name="id">The group or user id or to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no group or user was found for the given id.</exception>
        /// <returns><see cref="AdObject"/></returns>
        AdObject GetUserOrGroupAsync(string id);

        /// <summary>
        /// Checks the validity of multiple users by a query to the Microsoft Graph API and returns them accordingly in the form of an AdUserDto.
        /// In the first step a batch request is created based on the given email addresses. Then all reuqests are evaluated and only responses
        /// with the status OK are mapped accordingly.All other result types are ignored and mapped to invalid.
        ///
        /// Invalid users will be cached for the configured expiration time and not requested from ms graph.
        /// </summary>
        /// <param name="emailList">user email-adresses to check</param>
        /// <exception cref="ArgumentException">if the email pattern doesn't match</exception>
        Task<IEnumerable<AdUserDto>> CheckUsersValidityAsync(ISet<string> emailList);
    }
}
