using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Graph.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;
using Common.DataModels.TransferObjects;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class MessageService : GenericService<Message, int>, IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserService _userService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IColidEntrySubscriptionService _colidEntrySubscriptionService;
        private readonly IActiveDirectoryService _activeDirectoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepo, IUserService userService, IMessageTemplateService messageTemplateService,
            IColidEntrySubscriptionService colidEntrySubscriptionService, IActiveDirectoryService activeDirectoryService, IMapper mapper, ILogger<MessageService> logger) : base(messageRepo)
        {
            _messageRepo = messageRepo;
            _userService = userService;
            _messageTemplateService = messageTemplateService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _activeDirectoryService = activeDirectoryService;
            _mapper = mapper;
            _logger = logger;
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

                var _ = await _userService.AddMessageAsync(user.Id, message);
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
                    .Replace("%INVALID_USERS%",  string.Join(", ", entry.InvalidUsers));

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
            var unreadMessagesDto = new List<MessageUserDto>();
            var unreadMessages = _messageRepo.GetUnreadMessagesToSend();
            foreach (var msg in unreadMessages)
            {
                var msgDto = _mapper.Map<MessageUserDto>(msg);
                msgDto.UserId = msg.User.Id;
                msgDto.UserEmail = msg.User.EmailAddress;
                unreadMessagesDto.Add(msgDto);
            }

            return unreadMessagesDto;
        }
    }
}
