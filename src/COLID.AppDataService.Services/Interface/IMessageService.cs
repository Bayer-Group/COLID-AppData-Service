using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service to handle all message related operations.
    /// </summary>
    public interface IMessageService : IServiceBase<Message>
    {
        /// <summary>
        /// This function will create messages for all users, that subscribed to the colid pid uri,
        /// which is in the given dto. Also the label will be used to create a correct message, based
        /// on the template for colid entry subscriptions from the according entity.
        ///
        /// The total amount of created messages will be returned. In case that no user subscribed
        /// to the given uri, a 0 will be returned.
        /// </summary>
        /// <param name="colidEntryDto">pid uri to search for and label to replace</param>
        /// <exception cref="EntityNotFoundException">if not message template was available</exception>
        Task<int> CreateUpdateMessagesForColidEntrySubscriptions(ColidEntryDto colidEntryDto);

        /// <summary>
        /// By this, all users that subscribed to the colid Entry, that match on the uri in the given dto will be notified.
        /// First, all messages will be created for users, if at least one user subscribed to the given COLID entry.
        /// In the second step, all subscribed users will be notified about the deleted resource, which they subscribed to.
        /// And last, the subscription will be removed from the users.
        /// </summary>
        /// <param name="colidEntryDto">dto containing URL to delete all references to</param>
        /// <exception cref="EntityNotFoundException">if not message template was available</exception>
        Task<int> CreateDeleteMessagesAndRemoveColidEntrySubscriptions(ColidEntryDto colidEntryDto);

        /// <summary>
        /// With this function, a user is notified of invalid users for each colid Entry.
        /// In case that the recipient user hasn't been logged into the COLID editor yet, a new user will be created with
        /// his id (Guid) fetched from Azure AD. Afterwards, the template and message config will be fetched and used.
        /// </summary>
        /// <param name="colidEntryContactInvalidUsersDto">dto containing URL to delete all references to</param>
        /// <exception cref="EntityNotFoundException">if not message template was available</exception>
        Task CreateMessagesOfInvalidUsersForContact(ColidEntryContactInvalidUsersDto colidEntryContactInvalidUsersDto);

        /// <summary>
        /// Get a list of user related messages, that hasn't been read until now and where a SendOn
        /// value is set.
        /// </summary>
        IList<MessageUserDto> GetUnreadMessagesToSend();

        /// <summary>
        /// The method checks for which messages in the database the conditions for deleting the message exist.
        /// The conditions are that both the deletion date is reached and that the send or read date is not empty.
        /// </summary>
        void DeleteExpiredMessages();

        /// <summary>
        /// Sends a given Message to all users.
        /// <param name="message">Message dto containing subject and Body of the message to send</param>
        /// </summary>
        void SendMessageToAllUsers(BroadcastMessageDto message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributionEndpoints"></param>
        void DeleteByAdditionalInfo(List<Uri> distributionEndpoints);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributionEndpoints"></param>
        List<(Uri, DateTime)> GetByAdditionalInfo(List<Uri> distributionEndpoints);

        /// <summary>
        /// Used to notify user about invalid distribution endpoint
        /// </summary>
        /// <param name="message">message contain message text, email address and other information required to send email</param>
        void SendMessageToUser(DistributionEndpointMessageDto message);

        /// <summary>
        /// Send generic message to user
        /// </summary>
        /// <param name="message">Message to be sent</param>
        void SendGenericMessageToUser(MessageUserDto message);
    }
}
