using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all colid entry subscription related operations.
    /// </summary>
    public interface IColidEntrySubscriptionService : IServiceBase<ColidEntrySubscription>
    {
        /// <summary>
        /// Fetches a single colid entry subscription, identified by a given user id and colid pid uri within the dto.
        /// </summary>
        /// <param name="userId">the user id to search for</param>
        /// <param name="dto">the dto to search for (only colid pid uri used)</param>
        /// <exception cref="UriFormatException">If the given uri doesn't consist of a valid uri scheme</exception>
        /// <exception cref="EntityNotFoundException">in case that no subscription exists for a given user and uri</exception>
        ColidEntrySubscription GetOne(Guid userId, ColidEntrySubscriptionDto dto);

        /// <summary>
        /// Try to get all users which subscribed on the given colid pidUri and writes the result
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
        /// Fetches all Subscriptions and count the amount of subscribers per colid pid uri
        /// </summary>
        /// <param name="colidPidUris">list of COLID entry PID URIs</param>
        /// <return>list of PID URIs and their number of subscriptions</return>
        IList<ColidEntrySubscriptionAmountDto> GetColidPidUrisAndAmountSubscriptions(ISet<Uri> colidPidUris);
    }
}
