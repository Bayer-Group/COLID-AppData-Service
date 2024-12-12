using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Search;
using Common.DataModels.TransferObjects;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Services.Interfaces
{
    /// <summary>
    /// Service to handle all user related operations.
    /// </summary>
    public interface IUserService : IServiceBase<User>
    {
        Task<User> GetOneAsync(Guid userId);

        /// <summary>
        /// Create a new user from the given dto.
        /// </summary>
        /// <param name="userDto">the user to create</param>
        Task<User> Create(UserDto userDto);

        /// <summary>
        /// Create a new user asynchronously from the given dto.
        /// </summary>
        /// <param name="userDto">the user to create</param>
        Task<User> CreateAsync(UserDto userDto);

        /// <summary>
        /// Fetches the id for a user, identified by a given email address.
        /// </summary>
        /// <param name="emailAddress">the users email address to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user exists</exception>
        Task<Guid> GetIdByEmailAddressAsync(string emailAddress);

        /// <summary>
        /// Update the email address for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="email">the email to set</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> UpdateEmailAddressAsync(Guid userId, string email);

        /// <summary>
        /// Get the default consumer group for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user or consumer group was found for the given id</exception>
        Task<ConsumerGroup> GetDefaultConsumerGroupAsync(Guid userId);

        /// <summary>
        /// Process all users stored queries which execution date has expired
        /// </summary>
        /// <exception cref="EntityNotChangedException">in case that the storedquery couldn't be updated</exception>
        Task ProcessStoredQueries();

        /// <summary>
        /// Checks if a stored query's due date has been reached
        /// </summary>
        bool StoredQueryNeedsToBeEvaluated(StoredQuery storedQuery);

        /// <summary>
        /// Gets a List of all Pid Uris with the corresponding resource label which have been created or updated since last stored query execution date
        /// </summary>
        IList<UpdatedResourceStoredQueryDto> GetUpdatedResources(IList<SearchHit> hits, StoredQuery storedQuery);

        /// <summary>
        /// Performs a search for a given searchfilterDataMarketplace object in the searchservice
        /// </summary>  
        /// <exception cref="AuthenticationException">in case the token couldnt be validated</exception>
        /// <exception cref="HttpRequestException">in case the connection to the remote search service couldn't be established</exception>
        Task<SearchResultDTO> GetElasticSearchResult(SearchFilterDataMarketplace searchfilter);
        
        /// <summary>
        /// Notify user about created or updated resources since last execution of their storedquery
        /// </summary>  
        Task NotifyUserAboutUpdates(SearchFilterDataMarketplace sf, IList<UpdatedResourceStoredQueryDto> updatedResources);

        /// <summary>
        /// Create Hash for a given elasticsearch search result
        /// </summary>  
        string GetHashOfSearchResults(SearchResultDTO searchResult);

        /// <summary>
        /// Update the default consumer group value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="consumerGroup">the consumer group to set, identified by by the dto</param>
        /// <exception cref="EntityNotFoundException">in case that no user or consumer group was found for the given user</exception>
        Task<User> UpdateDefaultConsumerGroupAsync(Guid userId, ConsumerGroupDto consumerGroup);

        /// <summary>
        /// Remove the default consumer group value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> RemoveDefaultConsumerGroupAsync(Guid userId);

        /// <summary>
        /// Update the last login into data marketplace for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="time">the last login time</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found</exception>
        Task<User> UpdateLastLoginDataMarketplaceAsync(Guid userId, DateTime time);

        /// <summary>
        /// Update the last login into colid for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="time">the last login time</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found</exception>
        Task<User> UpdateLastLoginColidAsync(Guid userId, DateTime time);

        /// <summary>
        /// Update the last notification check time for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="time">the last login time</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found</exception>
        Task<User> UpdateLastTimeCheckedAsync(Guid userId, DateTime time);

        /// <summary>
        /// Update the showUserInformation flag for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="showUserInformationFlag">flag value</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found</exception>
        Task<User> UpdateShowUserInformationFlagAsync(Guid userId, bool showUserInformationFlag);

        /// <summary>
        /// Get the current editor search filter for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter was found</exception>
        Task<SearchFilterEditor> GetSearchFilterEditorAsync(Guid userId);

        /// <summary>
        /// Updates the colid search filter value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="jsonString">the search filter as json to set for colid</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        /// <exception cref="JsonReaderException">in case that the string can't be parsed as json</exception>
        Task<User> UpdateSearchFilterEditorAsync(Guid userId, JObject jsonString);

        /// <summary>
        /// Remove the current search filter for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter was found</exception>
        Task<User> RemoveSearchFilterEditorAsync(Guid userId);

        /// <summary>
        /// Get the default search filter data marketplace for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter data marketplace was found for the given id</exception>
        Task<SearchFilterDataMarketplace> GetDefaultSearchFilterDataMarketplaceAsync(Guid userId);

        /// <summary>
        /// Update the default search filter data marketplace value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="searchFilterId">the search filter data marketplace to set, identified by the given id</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter data marketplace was found for the given id's</exception>
        Task<User> UpdateDefaultSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId);

        /// <summary>
        /// Remove the default search filter data marketplace value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> RemoveDefaultSearchFilterDataMarketplaceAsync(Guid userId);

        /// <summary>
        /// Add a single data marketplace search filter to the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="searchFilter">the search filter to add</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found</exception>
        Task<User> AddSearchFilterDataMarketplaceAsync(Guid userId, SearchFilterDataMarketplaceDto searchFilter);

        /// <summary>
        /// Get a single data marketplace search filters for the user, that matches with the given id's.
        /// </summary>
        /// <param name="userId">the referencing user</param>
        /// <param name="searchFilterId">the referencing search filter</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filter was found</exception>
        Task<SearchFilterDataMarketplace> GetSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId);

        /// <summary>
        /// Get all data marketplace search filters for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<ICollection<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceAsync(Guid userId);

        /// <summary>
        /// Remove all data marketplace search filters for the user, that matches with the given data marker search filter id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="searchFilterId">the referencing search filter to remove</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<User> RemoveSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId);

        /// <summary> 
        /// Get all search filters from all users 
        /// </summary> 
        /// <returns></returns> 
        Task<ICollection<SearchFilterDataMarketplace>> GetAllSearchFiltersDataMarketplaceAsync();

        /// <summary>
        /// Get all subscribed data marketplace search filters of the user with the given Id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<ICollection<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync(Guid userId);

        /// <summary>
        /// Subscribe to a specific data marketplace search filter of the user with the given query settings.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="storedQueryDto">Stored Query parameter</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<SearchFilterDataMarketplace> AddStoredQueryToSearchFiltersDataMarketplaceAync(Guid userId, StoredQueryDto storedQueryDto);

        /// <summary>
        /// Remove a subscription of a specific data marketplace search filter of the user.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="SearchFilterDataMarketplaceId">Stored Query parameter</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<SearchFilterDataMarketplace> RemoveStoredQueryFromSearchFiltersDataMarketplaceAync(Guid userId, int SearchFilterDataMarketplaceId);

        /// <summary>
        /// Get all subscribed data market search filter of all users.
        /// </summary>
        Task<ICollection<User>> GetAllSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync();

        /// <summary>
        /// Get all subscribed data market search filter of all users.
        /// </summary>
        Task<Dictionary<string, int>> GetSearchFiltersDataMarketplaceCount();
        

        /// <summary>
        /// Get all user related subscription on COLID entries. The user will be identified by the given ID.
        /// </summary>
        /// <param name="userId">The user id to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<ICollection<ColidEntrySubscriptionDto>> GetColidEntrySubscriptionsAsync(Guid userId);

        Task<IEnumerable<ColidEntrySubscriptionDetailsDto>> GetLatestColidEntrySubscriptionsOfUserAsync(Guid userId);
        Task<IEnumerable<ColidEntrySubscriptionDetailsDto>> GetMostSubscribedColidEntrySubscriptions(int take = 5);

        /// <summary>
        /// Add the given colid entry to the collection of subscriptions per user.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="colidEntrySubscriptionDto">The subscription to append</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        /// <exception cref="EntityAlreadyExistsException">in case that the user is already subscribed to this colid entry</exception>
        Task<User> AddColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto);

        /// <summary>
        /// Removes the given colid entry to the collection of subscriptions per user.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="colidEntrySubscriptionDto">the subscription to append</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no subscription was found for the given userId and dto</exception>
        Task<User> RemoveColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto);

        /// <summary>
        /// Gets the current message config for a user, identified by the given id.
        /// By default, these values set initially for all users to:<br />
        /// - SendInterval: Weekly<br />
        /// - DeleteInterval: Monthly
        /// </summary>
        /// <param name="userId">the user to search for</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<MessageConfig> GetMessageConfigAsync(Guid userId);

        /// <summary>
        /// Update the message config for a user, identified by the given id.
        /// </summary>
        /// <param name="userId">the user to search for</param>
        /// <param name="messageConfigDto">the message config to set</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> UpdateMessageConfigAsync(Guid userId, MessageConfigDto messageConfigDto);

        /// <summary>
        /// Add a new message to the user asynchronously, identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="messageDto">the message to create as dto</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> AddMessageAsync(Guid userId, MessageDto messageDto);

        /// <summary>
        /// Add a new message to the user, identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="message">the message to create as Dto</param>
        User AddMessage(Guid userId, MessageDto message);

        /// <summary>
        /// Delete a message to the user, identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="messageId">the message to delete</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task DeleteMessageAsync(Guid userId, int messageId);

        /// <summary>
        /// Add multiple messages to the user, identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="messages">the messages to create as a list of dtos</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> AddMessagesAsync(Guid userId, ICollection<MessageDto> messages);

        /// <summary>
        /// Get all messages, which are stored within the user. This user will be identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<ICollection<MessageDto>> GetMessagesAsync(Guid userId);

        /// <summary>
        /// User-related messages can be marked as read, which will be accomplished by setting the
        /// ReadOn-flag to the current date.
        /// <b>NOTE:</b> If the message has already been marked in the past, no new date will be set and the
        /// old one will be returned within the message.
        /// </summary>
        /// <param name="userId">the user to seach for</param>
        /// <param name="messageIds">the message ids to mark as read</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user or message was found for the given ids</exception>
        Task<ICollection<MessageDto>> MarkMessagesAsReadAsync(Guid userId, ICollection<int> messageIds);

        /// <summary>
        /// User-related messages can be marked as read, which will be accomplished by setting the
        /// ReadOn-flag to the current date.
        /// <b>NOTE:</b> If the message has already been marked in the past, no new date will be set and the
        /// old one will be returned within the message.
        /// </summary>
        /// <param name="userId">the user to seach for</param>
        /// <param name="messageId">the message to mark as read</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user or message was found for the given ids</exception>
        Task<MessageDto> MarkMessageAsReadAsync(Guid userId, int messageId);

        /// <summary>
        /// User-related messages can also be marked as sent, which will be accomplished by setting the
        /// SendOn-flag to a specific date, dependent from the users message config values.
        /// <b>NOTE:</b> If a message got sent, the SendOn value will update to a future time, also dependent
        /// from the user specific message config.
        /// </summary>
        /// <param name="userId">the user to seach for</param>
        /// <param name="messageId">the message to mark as sent</param>
        /// <exception cref="ArgumentNullException">if the userId is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user or message was found for the given ids</exception>
        Task<MessageDto> MarkMessageAsSentAsync(Guid userId, int messageId);

        /// <summary>
        /// Creates an entry for user favorite list.
        /// </summary>
        /// <param name="userId">The user to add entry for</param>
        /// <param name="favoritesListDto">the favorites List piduris and name entries</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> AddFavoritesListAsync(Guid userId, FavoritesListDto favoritesListDto);

        /// <summary>
        /// Creates a favorite list entry for the user favorite list.
        /// </summary>
        /// <param name="userId">The user to add entry for</param>
        /// <param name="favoritesListId">the favorites list Id for which entry needs to be created</param>
        /// <param name="favoritesListDto">the favorites List piduris and name entries</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>

        /// <summary>
        /// Insert multiple entries into favorites lists.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntriesDto">the list id. PIDURIs of the entries to be added </param>
        /// <returns>A list containing an overview of FavoriteLists Names and its entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        Task<List<FavoritesList>> AddFavoritesListEntriesAsync(Guid userId, IList<FavoritesListEntriesDTO> favoritesListEntriesDto);

        Task<FavoritesList> AddFavoritesListEntryPerID(Guid userId, int favoritesListId, FavoritesListDto favoritesListDto);
        
        /// <summary>
        /// Fetches all user favorites lists.
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<List<FavoritesList>> GetFavoritesListsAsync(Guid userId);

        /// <summary>
        /// Fetches all favorites lists.
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetAllFavoritesListCount();

        /// <summary>
        /// Fetches all user favorites lists including only the IDs of the contained resources.
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<List<string>> GetFavoritesListPIDUris(Guid userId);

        /// <summary>
        /// Fetches user's favorite list including its contents with full details.
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="favoritesListId">The favorite list Id for</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<IDictionary<string, JObject>> GetFavoritesListDetails(Guid userId, int favoritesListId);

        /// <summary>
        /// Fetches the IDs of lists in which a specific resouce is in.
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="pidUri">The PID URI</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<List<int>> GetResourceFavoritesList(Guid userId, string pidUri);

        /// <summary>
        /// Edits a user Favorite list (e.g. for changing the name)
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="favoritesListId">The Favorite List Id to be changed</param>
        /// <param name="favoritesListDto">The new name to be updated will be part of the DTO</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<FavoritesList> SetFavoritesListName(Guid userId, int favoritesListId, FavoritesListDto favoritesListDto);

        /// <summary>
        /// Edits a user Favorite list Entry (e.g. for changing the personal Note)
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="favoritesListEntryId">The Favorite List Entry Id to be changed</param>
        /// <param name="favoritesListDto">The personal note to be updated will be part of the DTO</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<FavoritesListEntry> SetFavoritesListEntryNote(Guid userId, int favoritesListEntryId, FavoritesListDto favoritesListDto);

        /// <summary>
        /// Deletes a user Favorite list
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="favoritesListId">The Favorite List Id to be removed</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> RemoveFavoritesListAsync(Guid userId, int favoritesListId);

        /// <summary>
        /// Removes a user Favorite list Entry
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <param name="favoritesListEntryId">The Favorite List Entry Id to be removed</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<List<FavoritesList>> RemoveFavoritesEntryAsync(Guid userId, int favoritesListEntryId);

        /// <summary>
        /// Delete multiple entries into favorites lists.
        /// </summary>
        /// <param name="userId">unique user id</param>
        /// <param name="favoritesListEntriesId">the list ids entries to be removed </param>
        /// <returns>A list containing an overview of FavoriteLists Names and its remaining entries</returns>
        /// <response code="200">A list containing an overview of FavoriteLists Names and its entries</response>
        /// <response code="404">If entity is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        Task<List<FavoritesList>> RemoveFavoritesListEntriesAsync(Guid userId, IList<int> favoritesListEntriesId);
    }
}
