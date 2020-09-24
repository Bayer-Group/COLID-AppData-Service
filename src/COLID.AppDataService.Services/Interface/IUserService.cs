using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all user related operations.
    /// </summary>
    public interface IUserService : IGenericService<User, Guid>
    {
        /// <summary>
        /// Create a new user from the given dto.
        /// </summary>
        /// <param name="userDto">the user to create</param>
        User Create(UserDto userDto);

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
        /// Updates the data marketplace search filter value for the user, that matches with the given id..
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="dataMarketplaceFilters">the search filter to set for data marketplace</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> UpdateSearchFiltersDataMarketplaceAsync(Guid userId, ICollection<SearchFilterDataMarketplace> dataMarketplaceFilters);

        /// <summary>
        /// Remove all data marketplace search filters for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="searchFilterId">the referencing search filter to remove</param>
        /// <exception cref="EntityNotFoundException">in case that no user or search filters were found</exception>
        Task<User> RemoveSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId);

        /// <summary>
        /// Updates the colid resource subscription value for the user that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="colidEntrySubscriptions">the colid resource subscription to set</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task UpdateColidEntrySubscriptionsAsync(Guid userId, ICollection<ColidEntrySubscription> colidEntrySubscriptions);

        /// <summary>
        /// Get all user related subscription on COLID entries. The user will be identified by the given ID.
        /// </summary>
        /// <param name="userId">The user id to search for</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<ICollection<ColidEntrySubscriptionDto>> GetColidEntrySubscriptionsAsync(Guid userId);

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
        /// Add a new message to the user, identified by the given id.
        /// </summary>
        /// <param name="userId">The user to search for</param>
        /// <param name="messageDto">the message to create as dto</param>
        /// <exception cref="ArgumentNullException">if the userId or dto is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        Task<User> AddMessageAsync(Guid userId, MessageDto messageDto);

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
        /// Updates the stored queries value for the user, that matches with the given id.
        /// </summary>
        /// <param name="userId">the referencing user to update</param>
        /// <param name="storedQueries">the stored queries to set</param>
        /// <exception cref="EntityNotFoundException">in case that no user was found for the given id</exception>
        void UpdateStoredQueriesAsync(Guid userId, ICollection<StoredQuery> storedQueries);
    }
}
