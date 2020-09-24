using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using Microsoft.Graph;

namespace COLID.AppDataService.Services.Graph.Interface
{
    public interface IRemoteGraphService
    {
        /// <summary>
        /// Checks the validity of multiple users by a query to the Microsoft Graph API and returns them accordingly in the form of an AdUserDto.
        /// In the first step a batch request is created based on the given email addresses. Then all reuqests are evaluated and only responses
        /// with the status OK are mapped accordingly.All other result types are ignored and mapped to invalid.
        /// </summary>
        /// <param name="adUserEmailSet">The user email-adresses to check</param>
        Task<IList<AdUserDto>> CheckUsersValidityAsync(ISet<string> adUserEmailSet);

        /// <summary>
        /// Find one user by a given id and returns it.
        /// </summary>
        /// <param name="id">The user id to search for. The id can be the object id or email address</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id.</exception>
        /// <returns><see cref="User"/></returns>
        Task<User> GetUserAsync(string id);

        /// <summary>
        /// Find users by a given search term and returns them.
        /// </summary>
        /// <param name="query">The term to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no user was found for the given id, or the user has no manager.</exception>
        /// <returns>List of <see cref="User"/></returns>
        Task<IList<User>> FindActiveUserAsync(string query);

        /// <summary>
        /// Find the manager id of a user by a given id and returns it.
        /// </summary>
        /// <param name="id">The user id to search for. The id can be the object id or email address.</param>
        /// <exception cref="EntityNotFoundException">In case that no user was found for the given id, or the user has no manager.</exception>
        /// <returns>Object identifier of user's manager</returns>
        Task<string> GetManagerIdOfUserAsync(string id);

        /// <summary>
        /// Find one group by a given id and returns it.
        /// </summary>
        /// <param name="id">The user id to search for. The id can only be the email address.</param>
        /// <exception cref="EntityNotFoundException">In case that no group was found for the given id.</exception>
        /// <returns><see cref="Group"/></returns>
        Task<Group> GetGroupAsync(string id);

        /// <summary>
        /// Find groups by a given search term and returns them.
        /// </summary>
        /// <param name="query">The term to search for.</param>
        /// <exception cref="EntityNotFoundException">In case that no group was found for the given search term.</exception>#
        /// <returns>List of <see cref="Group"/></returns>
        Task<IList<Group>> FindGroupAsync(string query);
    }
}
