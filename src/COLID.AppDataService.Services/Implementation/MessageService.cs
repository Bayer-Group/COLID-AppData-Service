using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;
using Common.DataModels.TransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class MessageService : ServiceBase<Message>, IMessageService
    {
        private readonly IUserService _userService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IColidEntrySubscriptionService _colidEntrySubscriptionService;
        private readonly IActiveDirectoryService _activeDirectoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageService> _logger;
        private readonly IConfiguration _configuration;

        public MessageService(IGenericRepository repo, IUserService userService, IMessageTemplateService messageTemplateService,
            IColidEntrySubscriptionService colidEntrySubscriptionService, IActiveDirectoryService activeDirectoryService, IMapper mapper, ILogger<MessageService> logger,
            IConfiguration configuration) : base(repo)
        {
            _userService = userService;
            _messageTemplateService = messageTemplateService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _activeDirectoryService = activeDirectoryService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<int> CreateUpdateMessagesForColidEntrySubscriptions(ColidEntryDto colidEntryDto)
        {
            if (!_colidEntrySubscriptionService.TryGetAllUsers(colidEntryDto.ColidPidUri, out var colidEntrySubscribedUsers))
            {
                // no one subscribed to the given colid pidUri
                return 0;
            }

            var amountUsersNotified = await CreateMessagesForColidEntrySubscriptions(colidEntryDto, MessageType.ColidEntrySubscriptionUpdate, colidEntrySubscribedUsers);
            return amountUsersNotified;
        }

        public async Task<int> CreateDeleteMessagesAndRemoveColidEntrySubscriptions(ColidEntryDto colidEntryDto)
        {
            if (!_colidEntrySubscriptionService.TryGetAllUsers(colidEntryDto.ColidPidUri, out var colidEntrySubscribedUsers))
            {
                // no one subscribed to the given colid pidUri
                return 0;
            }

            var amountUsersNotified = await CreateMessagesForColidEntrySubscriptions(colidEntryDto, MessageType.ColidEntrySubscriptionDelete, colidEntrySubscribedUsers);

            // remove subscription from all users
            var subscriptionDto = new ColidEntrySubscriptionDto() { ColidPidUri = colidEntryDto.ColidPidUri };
            _colidEntrySubscriptionService.Delete(subscriptionDto);

            return amountUsersNotified;
        }

        private async Task<int> CreateMessagesForColidEntrySubscriptions(ColidEntryDto colidEntryDto, MessageType messageType, IList<User> colidEntrySubscribedUsers)
        {
            var colidPidUri = colidEntryDto.ColidPidUri.AbsoluteUri;
            var colidLabel = colidEntryDto.Label;
            var messageTemplate = _messageTemplateService.GetOne(messageType);

            foreach (var user in colidEntrySubscribedUsers)
            {
                var messageConfigForUser = await _userService.GetMessageConfigAsync(user.Id);

                var subject = messageTemplate.Subject.Replace("%COLID_PID_URI%", colidPidUri).Replace("%COLID_LABEL%", colidLabel);
                var body = messageTemplate.Body.Replace("%COLID_PID_URI%", colidPidUri).Replace("%COLID_LABEL%", colidLabel);
                var message = CreateMessageDto(messageConfigForUser, subject, body);

                await _userService.AddMessageAsync(user.Id, message);
            }

            return colidEntrySubscribedUsers.Count;
        }

        public async Task CreateMessagesOfInvalidUsersForContact(ColidEntryContactInvalidUsersDto cec)
        {
            // It is only possible to send messages to users who are already logged in to COLID, because
            // they are assigned to the user. Therefore external users must be created separately beforehand.
            var userId = await CreateInitialUserForMessages(cec);

            var messageTemplate = _messageTemplateService.GetOne(MessageType.InvalidUserWarning);
            var messageConfigForUser = await _userService.GetMessageConfigAsync(userId);

            var messageList = new Collection<MessageDto>();
            foreach (var entry in cec.ColidEntries)
            {
                var subject = messageTemplate.Subject.Replace("%COLID_PID_URI%", entry.PidUri.OriginalString).Replace("%COLID_LABEL%", entry.Label);
                var body = messageTemplate.Body
                    .Replace("%COLID_PID_URI%", entry.PidUri.OriginalString)
                    .Replace("%COLID_LABEL%", entry.Label)
                    .Replace("%INVALID_USERS%", string.Join(", ", entry.InvalidUsers));

                var message = CreateMessageDto(messageConfigForUser, subject, body);
                messageList.Add(message);
            }

            var _ = await _userService.AddMessagesAsync(userId, messageList);
        }

        private static MessageDto CreateMessageDto(MessageConfig messageConfigForUser, string subject, string body)
        {
            var sendOn = DateTime.Now.CalculateByInterval(messageConfigForUser.SendInterval);
            var deleteOn = DateTime.Now.CalculateByInterval(messageConfigForUser.DeleteInterval);
            if (sendOn != null)
            {
                deleteOn = sendOn.Value.CalculateByInterval(messageConfigForUser.DeleteInterval);
            }

            var messageDto = new MessageDto { Subject = subject, Body = body, SendOn = sendOn, DeleteOn = deleteOn };
            return messageDto;
        }

        private async Task<Guid> CreateInitialUserForMessages(ColidEntryContactInvalidUsersDto cec)
        {
            try
            {
                return await _userService.GetIdByEmailAddressAsync(cec.ContactMail);
            }
            catch (EntityNotFoundException)
            {
                // in case, that the user doesn't exist within appdata service, create
                var adUser = await _activeDirectoryService.GetUserAsync(cec.ContactMail);
                var userDto = new UserDto
                {
                    Id = new Guid(adUser.Id),
                    EmailAddress = adUser.Mail
                };
                var user = await _userService.CreateAsync(userDto);
                return user.Id;
            }
        }

        public IList<MessageUserDto> GetUnreadMessagesToSend()
        {
            Expression<Func<Message, bool>> unreadMessagesFilter = m => !m.ReadOn.HasValue
                                                                        && m.SendOn.HasValue
                                                                        && m.SendOn < DateTime.UtcNow;
            try
            {
                var unreadMessages = Get(unreadMessagesFilter, null, nameof(Message.User))
                    .OrderBy(m => m.User?.Id) // Required, because users can be null ...
                    .ToList();

                var unreadMessagesDto = new List<MessageUserDto>();
                foreach (var msg in unreadMessages)
                {
                    var msgDto = _mapper.Map<MessageUserDto>(msg);
                    msgDto.UserId = msg.User.Id;
                    msgDto.UserEmail = msg.User.EmailAddress;
                    unreadMessagesDto.Add(msgDto);
                }

                return unreadMessagesDto;
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation("No messages found, that should be deleted.", ex);
                return new List<MessageUserDto>();
            }

        }

        public void DeleteExpiredMessages()
        {
            Expression<Func<Message, bool>> expiredMessageFilter = m => m.DeleteOn < DateTime.UtcNow
                                                                   && (m.ReadOn.HasValue || m.SendOn.HasValue);
            try
            {
                var messagesToDelete = Get(expiredMessageFilter).ToList();
                if (messagesToDelete.Any())
                {
                    DeleteRange(messagesToDelete);
                    Save();
                }
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation("No messages found, that should be deleted.", ex);
            }
        }

        public void SendMessageToAllUsers(BroadcastMessageDto message)
        {
            Guard.IsNotNull(message);
            Guard.IsNotEmpty(message.Subject);
            Guard.IsNotEmpty(message.Body);

            // users cant be changed at this level! They will be fetched again in foreach and changed there
            var readOnlyUsers = _userService.GetAll(null, nameof(User.MessageConfig), null, null, true);

            foreach (var user in readOnlyUsers)
            {
                var messageForUser = CreateMessageDto(user.MessageConfig, message.Subject, message.Body);
                _userService.AddMessage(user.Id, messageForUser);
            }
        }
        /// <summary>
        /// Used to notify user about invalid distribution endpoint
        /// </summary>
        /// <param name="message">message contain message text, email address and other information required to send email</param>
        public void SendMessageToUser(DistributionEndpointMessageDto message)
        {
            Guard.IsNotNull(message);
            Guard.IsValidEmail(message.UserEmail);

            DebugMode(message);

            var messageTemplate = _messageTemplateService.GetOne(MessageType.InvalidDistributionEndpointTargetUri);
            var user = _userService.GetOne(u => u.EmailAddress.Equals(message.UserEmail), nameof(User.MessageConfig));
            var subject = messageTemplate.Subject
                .Replace("%COLID_LABEL%", message.ResourceLabel);

            var body = messageTemplate.Body
                .Replace("%COLID_PID_URI%", message.ColidEntryPidUri)
                .Replace("%DISTRIBUTION_ENDPOINT%", message.DistributionEndpoint.ToString())
                .Replace("%COLID_LABEL%", message.ResourceLabel);

            var messageForUser = CreateMessageDto(user.MessageConfig, subject, body);
            messageForUser.AdditionalInfo = string.Format("DistributionEndpoint: {0}", message.DistributionEndpointPidUri);
            _userService.AddMessage(user.Id, messageForUser);

            _logger.LogInformation($"Message added in the message list for {message.DistributionEndpointPidUri}");
        }

        private void DebugMode(DistributionEndpointMessageDto message)
        {
            try
            {
                if (_configuration.GetValue<bool>("DebugModeEndpointTest"))
                {
                    var emailAddress = _configuration.GetValue<string>("EndpointTestEmailAddress");
                    Guard.IsValidEmail(emailAddress);
                    message.UserEmail = emailAddress;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributionEndpoints"></param>
        public void DeleteByAdditionalInfo(List<Uri> distributionEndpoints)
        {
            Guard.IsNotNull(distributionEndpoints);

            var matchingMessages = GetAll()
                .AsEnumerable()
                  .Where(x =>
                    x.AdditionalInfo != null &&
                    distributionEndpoints.Any(endpoint => x.AdditionalInfo.Contains(endpoint.ToString()))
                 );

            foreach (var message in matchingMessages)
            {
                _logger.LogInformation($"Message for {message.AdditionalInfo} marked for deletion");
                message.DeleteOn = DateTime.UtcNow;
                message.ReadOn = DateTime.UtcNow;
                Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributionEndpoints"></param>
        /// <returns></returns>
        public List<(Uri, DateTime)> GetByAdditionalInfo(List<Uri> distributionEndpoints)
        {
            Guard.IsNotNull(distributionEndpoints);

            var matchingMessages = GetAll()
                 .AsEnumerable()
                 .Where(x =>
                    x.AdditionalInfo != null &&
                    distributionEndpoints.Any(endpoint => x.AdditionalInfo.Contains(endpoint.ToString()))
                 )
                 .Select(x =>
                 (
                     distributionEndpoints.Single(endpoint => x.AdditionalInfo.Contains(endpoint.ToString())) //get field from free text and remove unwanted space and text
                     , x.CreatedAt.Value
                 )).ToList();

            _logger.LogInformation($"Found {matchingMessages?.Count()} messages for validation with current status");
            return matchingMessages;
        }

        /// <summary>
        /// Send generic message to user
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public void SendGenericMessageToUser(MessageUserDto message)
        {
            Guard.IsNotNull(message);
            Guard.IsNotEmpty(message.Subject);
            Guard.IsNotEmpty(message.Body);
            Guard.IsValidEmail(message.UserEmail);

            var user = _userService.GetOne(u => u.EmailAddress.Equals(message.UserEmail), nameof(User.MessageConfig));
            var messageForUser = CreateMessageDto(user.MessageConfig, message.Subject, message.Body);
            _userService.AddMessage(user.Id, messageForUser);
        }
    }
}
