using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all colid entry subscription related operations.
    /// </summary>
    public interface IColidEntrySubscriptionRepository : IGenericRepository<ColidEntrySubscription, int>
    {
        /// <summary>
        /// Fetches a single colid entry subscription, identified by a given user id and COLID pid uri.
        /// </summary>
        /// <param name="userId">the user id to search for</param>
        /// <param name="colidPidUri">the colid pid uri to search for</param>
        /// <exception cref="UriFormatException">If the given uri doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityNotFoundException">in case that no subscription exists for a given user and uri</exception>
        ColidEntrySubscription GetOne(Guid userId, Uri colidPidUri);

        /// <summary>
        /// Try to get all users which subscribed on the given COLID pid uri and writes the result
        /// into the out param. If no users were found, the list will be empty.
        /// </summary>
        /// <param name="colidPidUri">The pid uri to search for</param>
        /// <param name="subscribedUsers">the subscribed users, otherwise empty</param>
        /// <return>true if entity was found, otherwise false</return>
        bool TryGetAllUsers(Uri colidPidUri, out IList<User> subscribedUsers);

        /// <summary>
        /// Based on the given DTO, all colid entry subscriptions will be deleted, that match with this object (url).
        /// If three users subscribed to the same colid entry, and this entry uri will be passed by the dto, the
        /// subscription will be removed for all three users.
        /// </summary>
        /// <param name="dto">dto containing URL to delete all references to</param>
        void Delete(ColidEntrySubscriptionDto dto);

        /// <summary>
        /// Fetches all Subscriptions and count the amount of subscribers per COLID pid uri.
        /// </summary>
        IList<ColidEntrySubscriptionAmountDto> CountSubsciptionsPerColidPidUri();

    }
}
