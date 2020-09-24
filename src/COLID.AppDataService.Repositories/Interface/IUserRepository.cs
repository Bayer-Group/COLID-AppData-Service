using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository to handle all user related operations.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
        /// <summary>
        /// Fetches the id for a user, identified by a given email address.
        /// </summary>
        /// <param name="emailAddress">the users email address to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user exists</exception>
        Task<Guid> GetIdByEmailAddressAsync(string emailAddress);

        /// <summary>
        /// Fetches the default consumer group for a user, identified by a given user id.
        /// </summary>
        /// <param name="userId">the user id to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user or consumer group exists</exception>
        Task<ConsumerGroup> GetDefaultConsumerGroupAsync(Guid userId);

        /// <summary>
        /// Fetches the editor search filter for a user, identified by a given user id.
        /// </summary>
        /// <param name="userId">the user id to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter exists</exception>
        Task<SearchFilterEditor> GetSearchFilterEditorAsync(Guid userId);

        /// <summary>
        /// Fetches the data marketpalce search filters for a user, identified by a given user id.
        /// </summary>
        /// <param name="userId">the user id to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters exists</exception>
        Task<IList<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceAsync(Guid userId);

        /// <summary>
        /// Get a single data marketplace search filters for the user, that matches with the given id's.
        /// </summary>
        /// <param name="userId">the referencing user</param>
        /// <param name="searchFilterId">the referencing search filter</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter was found</exception>
        Task<SearchFilterDataMarketplace> GetSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId);
    }
}
