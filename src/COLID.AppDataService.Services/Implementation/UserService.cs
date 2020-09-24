using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Services.Implementation
{
    public class UserService : GenericService<User, Guid>, IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConsumerGroupService _consumerGroupService;
        private readonly IGenericService<SearchFilterDataMarketplace, int> _searchFilterDataMarketplaceService;
        private readonly IColidEntrySubscriptionService _colidEntrySubscriptionService;
        private readonly IGenericService<StoredQuery, int> _storedQueryService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo,
            IConsumerGroupService consumerGroupService,
            IGenericService<SearchFilterDataMarketplace, int> searchFilterDataMarketplaceService,
            IColidEntrySubscriptionService colidEntrySubscriptionService,
            IGenericService<StoredQuery, int> storedQueryService,
            IMapper mapper, ILogger<UserService> logger) : base(userRepo)
        {
            _userRepo = userRepo;
            _consumerGroupService = consumerGroupService;
            _searchFilterDataMarketplaceService = searchFilterDataMarketplaceService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _storedQueryService = storedQueryService;
            _mapper = mapper;
            _logger = logger;
        }

        public User Create(UserDto userDto)
        {
            var userEntity = CheckAndPrepareUserEntityForCreate(userDto);
            return _userRepo.Create(userEntity);
        }

        public async Task<User> CreateAsync(UserDto userDto)
        {
            var userEntity = CheckAndPrepareUserEntityForCreate(userDto);
            return await _userRepo.CreateAsync(userEntity);
        }

        private User CheckAndPrepareUserEntityForCreate(UserDto userDto)
        {
            Guard.IsNotNull(userDto);
            Guard.IsValidEmail(userDto.EmailAddress);

            if (_userRepo.TryGetOne(userDto.Id, out var entity))
            {
                throw new EntityAlreadyExistsException("Couldn't create a new user, because a user with the id already exists", entity);
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.MessageConfig = new MessageConfig()
            {
                SendInterval = Common.Enums.SendInterval.Weekly,
                DeleteInterval = Common.Enums.DeleteInterval.Monthly
            };

            return userEntity;
        }

        public async Task<Guid> GetIdByEmailAddressAsync(string emailAddress)
        {
            Guard.IsValidEmail(emailAddress);
            var userId = await _userRepo.GetIdByEmailAddressAsync(emailAddress);
            return userId;
        }

        public async Task<User> UpdateEmailAddressAsync(Guid userId, string email)
        {
            Guard.IsValidEmail(email);
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.EmailAddress = email;
            return _userRepo.Update(userEntity);
        }


        #region [Default Consumer Group]

        public async Task<ConsumerGroup> GetDefaultConsumerGroupAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var cg = await _userRepo.GetDefaultConsumerGroupAsync(userId);
            return cg;
        }

        public async Task<User> UpdateDefaultConsumerGroupAsync(Guid userId, ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(userId);
            Guard.IsValidUri(consumerGroupDto.Uri);

            var consumerGroup = _consumerGroupService.GetOne(consumerGroupDto.Uri);
            var userEntity = await _userRepo.GetOneAsync(userId);

            userEntity.DefaultConsumerGroup = consumerGroup;
            return _userRepo.Update(userEntity);
        }

        public async Task<User> RemoveDefaultConsumerGroupAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.DefaultConsumerGroup = null;

            return _userRepo.UpdateReference(userEntity, u => u.DefaultConsumerGroup);
        }

        #endregion [Default Consumer Group]

        #region [Last Login fields]

        public async Task<User> UpdateLastLoginDataMarketplaceAsync(Guid userId, DateTime time)
        {
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.LastLoginDataMarketplace = time;
            return _userRepo.Update(userEntity);
        }

        public async Task<User> UpdateLastLoginColidAsync(Guid userId, DateTime time)
        {
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.LastLoginEditor = time;
            return _userRepo.Update(userEntity);
        }

        public async Task<User> UpdateLastTimeCheckedAsync(Guid userId, DateTime time)
        {
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.LastTimeChecked = time;
            return _userRepo.Update(userEntity);
        }

        #endregion [Last Login fields]

        #region [SearchFilter Editor]

        public async Task<SearchFilterEditor> GetSearchFilterEditorAsync(Guid userId)
        {
            var cg = await _userRepo.GetSearchFilterEditorAsync(userId);
            return cg;
        }

        public async Task<User> UpdateSearchFilterEditorAsync(Guid userId, JObject jsonString)
        {
            var entity = await _userRepo.GetOneAsync(userId);

            var searchFilterEditor = new SearchFilterEditor
            {
                FilterJson = jsonString
            };

            entity.SearchFilterEditor = searchFilterEditor;
            return _userRepo.Update(entity);
        }

        public async Task<User> RemoveSearchFilterEditorAsync(Guid userId)
        {
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.SearchFilterEditor = null;
            return _userRepo.UpdateReference(userEntity, u => u.SearchFilterEditor);
        }

        #endregion [SearchFilter Editor]

        #region [Default SearchFilter Data Marketplace]

        public async Task<SearchFilterDataMarketplace> GetDefaultSearchFilterDataMarketplaceAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var user = await _userRepo.GetOneAsync(userId);
            var dsf = user.SearchFiltersDataMarketplace.FirstOrDefault(e => e.Id.Equals(user.DefaultSearchFilterDataMarketplace));
            if (dsf == null)
            {
                throw new EntityNotFoundException($"No default data marketplace search filter found for user {userId}", userId.ToString());
            }
            return dsf;
        }

        public async Task<User> UpdateDefaultSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId)
        {
            Guard.IsNotNull(userId);
            Guard.IsPositiveInteger(searchFilterId);
            // TODO CK: Build searchfilterdmp Repository for all users and fetch Filters from there. Check if Exists, TryGetOne, etc.

            var userEntity = await _userRepo.GetOneAsync(userId);
            if (userEntity.SearchFiltersDataMarketplace.FirstOrDefault(sf => sf.Id.Equals(searchFilterId)).IsEmpty())
            {
                throw new EntityNotFoundException($"No data marketplace search filter with the id {searchFilterId} for the user {userId} exists");
            }

            userEntity.DefaultSearchFilterDataMarketplace = searchFilterId;
            return _userRepo.Update(userEntity);
        }

        public async Task<User> RemoveDefaultSearchFilterDataMarketplaceAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var userEntity = await _userRepo.GetOneAsync(userId);
            userEntity.DefaultSearchFilterDataMarketplace = null;

            return _userRepo.Update(userEntity);
        }

        #endregion [Default SearchFilter Data Marketplace]

        #region [SearchFilter Data Marketplace]

        public async Task<SearchFilterDataMarketplace> GetSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId)
        {
            Guard.IsNotNull(userId, searchFilterId);
            var searchFilter = await _userRepo.GetSearchFilterDataMarketplaceAsync(userId, searchFilterId);
            return searchFilter;
        }

        public async Task<ICollection<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var searchFiltersDataMarketplace = await _userRepo.GetSearchFiltersDataMarketplaceAsync(userId);
            return searchFiltersDataMarketplace;
        }

        public async Task<User> AddSearchFilterDataMarketplaceAsync(Guid userId, SearchFilterDataMarketplaceDto searchFilterDto)
        {
            Guard.IsNotNull(userId, searchFilterDto);
            var entity = await _userRepo.GetOneAsync(userId);
            var searchFilter = _mapper.Map<SearchFilterDataMarketplace>(searchFilterDto);
            searchFilter.User = entity;
            entity.SearchFiltersDataMarketplace ??= new Collection<SearchFilterDataMarketplace>();
            entity.SearchFiltersDataMarketplace.Add(searchFilter);
            return _userRepo.Update(entity);
        }

        public async Task<User> UpdateSearchFiltersDataMarketplaceAsync(Guid userId, ICollection<SearchFilterDataMarketplace> dataMarketplaceFilters)
        {
            Guard.IsNotNull(userId);
            Guard.IsNotNullOrEmpty(dataMarketplaceFilters);
            var entity = await _userRepo.GetOneAsync(userId);
            entity.SearchFiltersDataMarketplace = dataMarketplaceFilters;
            return _userRepo.Update(entity);
        }

        public async Task<User> RemoveSearchFilterDataMarketplaceAsync(Guid userId, int id)
        {
            Guard.IsNotNull(userId, id);
            _searchFilterDataMarketplaceService.Delete(id);
            var entity = await _userRepo.GetOneAsync(userId);
            return entity;
        }

        #endregion [SearchFilter Data Marketplace]

        #region [Colid Entry Subscriptions]

        public async Task<User> AddColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(userId, colidEntrySubscriptionDto);
            var entity = await _userRepo.GetOneAsync(userId);
            var colidEntrySubscription = _mapper.Map<ColidEntrySubscription>(colidEntrySubscriptionDto);
            colidEntrySubscription.User = entity;
            entity.ColidEntrySubscriptions ??= new Collection<ColidEntrySubscription>();

            CheckIfUserIsAlreadySubscribedToColidPidUri(entity, colidEntrySubscription);

            entity.ColidEntrySubscriptions.Add(colidEntrySubscription);
            return _userRepo.Update(entity);
        }

        private static void CheckIfUserIsAlreadySubscribedToColidPidUri(User entity, ColidEntrySubscription colidEntrySubscription)
        {
            var existingSubscription = entity.ColidEntrySubscriptions.Where(ces => colidEntrySubscription.ColidPidUri.AbsoluteUri == ces.ColidPidUri.AbsoluteUri).FirstOrDefault();
            if (existingSubscription.IsNotEmpty())
            {
                throw new EntityAlreadyExistsException("The user is already subscribed to the COLID entry.", existingSubscription);
            }
        }

        public async Task<ICollection<ColidEntrySubscriptionDto>> GetColidEntrySubscriptionsAsync(Guid userId)
        {
            var entity = await _userRepo.GetOneAsync(userId);
            var resultDto = _mapper.Map<ICollection<ColidEntrySubscriptionDto>>(entity.ColidEntrySubscriptions);

            return resultDto;
        }

        public async Task UpdateColidEntrySubscriptionsAsync(Guid userId, ICollection<ColidEntrySubscription> colidEntrySubscriptions)
        {
            var entity = await _userRepo.GetOneAsync(userId);
            entity.ColidEntrySubscriptions = colidEntrySubscriptions;
            _userRepo.Update(entity);
        }

        public async Task<User> RemoveColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(userId, colidEntrySubscriptionDto);
            var colidEntrySubscription = _colidEntrySubscriptionService.GetOne(userId, colidEntrySubscriptionDto);
            _colidEntrySubscriptionService.Delete(colidEntrySubscription);
            var entity = await _userRepo.GetOneAsync(userId);
            return entity;
        }

        #endregion [Colid Entry Subscriptions]

        #region [Message Config]

        public async Task<MessageConfig> GetMessageConfigAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var entity = await _userRepo.GetOneAsync(userId);
            if (entity.MessageConfig == null)
            {
                throw new EntityNotFoundException($"No message config for user {entity.Id} exists");
            }
            return entity.MessageConfig;
        }

        // check if the given message config is valid, if the values changed and update existing messages for the changed entity
        // pretends, that a message config per user exists
        public async Task<User> UpdateMessageConfigAsync(Guid userId, MessageConfigDto messageConfigDto)
        {
            Guard.IsNotNull(userId, messageConfigDto);
            CheckIfMessageConfigIsValid(messageConfigDto);

            var entity = await _userRepo.GetOneAsync(userId);

            var sendIntervalUpdated = entity.MessageConfig.SendInterval != messageConfigDto.SendInterval;
            var deleteIntervalUpdated = entity.MessageConfig.DeleteInterval != messageConfigDto.DeleteInterval;
            if (!sendIntervalUpdated && !deleteIntervalUpdated)
            {
                throw new EntityNotChangedException("Couldn't update the message config, because is hasn't changed");
            }

            entity.MessageConfig.SendInterval = messageConfigDto.SendInterval;
            entity.MessageConfig.DeleteInterval = messageConfigDto.DeleteInterval;

            if (entity.Messages == null || !entity.Messages.Any())
            {
                return _userRepo.Update(entity);
            }

            return UpdateMessagesSendOnAndDeleteOnByInterval(entity, sendIntervalUpdated, deleteIntervalUpdated);
        }

        /// <summary>
        ///  Based on the given arguments, all messages will be fetched and updated, if SendOn and/or DeleteOn was changed.
        /// </summary>
        private User UpdateMessagesSendOnAndDeleteOnByInterval(User entity, bool sendIntervalUpdated, bool deleteIntervalUpdated)
        {
            foreach (var message in entity.Messages)
            {
                if (sendIntervalUpdated && message.ReadOn == null)
                {
                    message.SendOn = DateTime.Now.CalculateByInterval(entity.MessageConfig.SendInterval);
                }

                if (deleteIntervalUpdated)
                {
                    message.DeleteOn = DateTime.Now.CalculateByInterval(entity.MessageConfig.DeleteInterval);
                }
            }

            return _userRepo.Update(entity);
        }

        /// <summary>
        /// Check if the given dto contains a send interval, which is higher or equal than the delete interval.
        /// </summary>
        private static void CheckIfMessageConfigIsValid(MessageConfigDto messageConfigDto)
        {
            int sendInt = (int)messageConfigDto.SendInterval;
            int deleteInt = (int)messageConfigDto.DeleteInterval;

            if (sendInt >= deleteInt)
            {
                throw new ArgumentException("The delete interval has to be higher than the send interval.");
            }
        }

        #endregion [Message Config]

        #region [Messages]

        public async Task<User> AddMessageAsync(Guid userId, MessageDto messageDto)
        {
            Guard.IsNotNull(userId, messageDto);
            var entity = await _userRepo.GetOneAsync(userId, true);
            var message = _mapper.Map<Message>(messageDto);
            message.User = entity;
            entity.Messages ??= new Collection<Message>();
            entity.Messages.Add(message);
            var user = _userRepo.Update(entity);

            return user;
        }

        public async Task DeleteMessageAsync(Guid userId, int messageId)
        {
            Guard.IsNotNull(userId, messageId);
            Guard.IsPositiveInteger(messageId);

            var entity = await _userRepo.GetOneAsync(userId, true);
            var message = entity.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message != null)
            {
                entity.Messages.Remove(message);
                var _ = _userRepo.Update(entity);
                return;
            }

            throw new EntityNotFoundException($"No message with the id {messageId} for user {userId} exists");
        }

        public async Task<User> AddMessagesAsync(Guid userId, ICollection<MessageDto> messages)
        {
            Guard.IsNotNull(userId);
            Guard.IsNotNullOrEmpty(messages);

            var entity = await _userRepo.GetOneAsync(userId, true);
            entity.Messages ??= new Collection<Message>();
            foreach (var message in messages)
            {
                var messageEntity = _mapper.Map<Message>(message);
                messageEntity.User = entity;
                entity.Messages.Add(messageEntity);
            }

            return _userRepo.UpdateCollectionReference(entity, u => u.Messages);
        }

        public async Task<ICollection<MessageDto>> GetMessagesAsync(Guid userId)
        {
            var entity = await _userRepo.GetOneAsync(userId);
            if (entity.Messages.IsNullOrEmpty())
            {
                return new Collection<MessageDto>();
            }

            return _mapper.Map<Collection<MessageDto>>(entity.Messages);
        }

        public async Task<ICollection<MessageDto>> MarkMessagesAsReadAsync(Guid userId, ICollection<int> messageIds)
        {
            var userEntity = await GetOneAsync(userId);

            foreach (var messageId in messageIds)
            {
                try
                {
                    var userMessage = GetMessageByUserAndId(userEntity, messageId);

                    if (userMessage.ReadOn != null)
                    {
                        continue;
                    }

                    userMessage.ReadOn = DateTime.Now;
                    userMessage.SendOn = null;
                }
                catch (EntityNotFoundException)
                {
                    // Ignore if a message was not found. 
                }
            }

            var updatedUser = Update(userEntity);
            var messages = updatedUser.Messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();

            return messages;
        }

        public async Task<MessageDto> MarkMessageAsReadAsync(Guid userId, int messageId)
        {
            var userEntity = await GetOneAsync(userId);
            var userMessage = GetMessageByUserAndId(userEntity, messageId);

            if (userMessage.ReadOn != null)
            {
                return _mapper.Map<MessageDto>(userMessage);
            }

            userMessage.ReadOn = DateTime.Now;
            userMessage.SendOn = null;
            var _ = Update(userEntity);

            return _mapper.Map<MessageDto>(userMessage);
        }

        public async Task<MessageDto> MarkMessageAsSentAsync(Guid userId, int messageId)
        {
            var userEntity = await GetOneAsync(userId);
            var userMessage = GetMessageByUserAndId(userEntity, messageId);

            if (userMessage.SendOn != null)
            {
                userMessage.SendOn = null;
                userMessage.ReadOn = DateTime.Now.LastDayOfWeek();

                Update(userEntity);
            }

            return _mapper.Map<MessageDto>(userMessage);
        }

        private Message GetMessageByUserAndId(User userEntity, int messageId)
        {
            Guard.IsPositiveInteger(messageId);
            var userMessage = userEntity.Messages?.FirstOrDefault(msg => msg.Id.Equals(messageId));
            if (userMessage.IsEmpty())
            {
                throw new EntityNotFoundException($"No message with the id {messageId} for user {userEntity.Id} exists");
            }

            return userMessage;
        }

        #endregion [Messages]

        public async void UpdateStoredQueriesAsync(Guid userId, ICollection<StoredQuery> storedQueries)
        {
            // Found under test: Do we really want to update the whole collection or add single SearchFilterDataMarketplace?
            var entity = await _userRepo.GetOneAsync(userId);
            entity.StoredQueries = storedQueries;
            _userRepo.Update(entity);
        }
    }
}
