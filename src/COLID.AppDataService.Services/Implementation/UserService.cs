using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.Graph.HashGenerator;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;
using COLID.AppDataService.Common.Search;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using COLID.Graph.HashGenerator.Services;
using System.Globalization;

namespace COLID.AppDataService.Services.Implementation
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IConsumerGroupService _consumerGroupService;
        private readonly IColidEntrySubscriptionService _colidEntrySubscriptionService;
        private readonly IRemoteSearchService _remoteSearchService;
        private readonly IMessageTemplateService _messageTemplateService;

        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        private readonly ISet<string> _includeProperties = new HashSet<string>
        {
            nameof(User.ColidEntrySubscriptions),
            nameof(User.DefaultConsumerGroup),
            nameof(User.SearchFiltersDataMarketplace),
            nameof(User.SearchFilterEditor),
            nameof(User.Messages),
            nameof(User.MessageConfig),
        };

        public UserService(IGenericRepository repo,
            IConsumerGroupService consumerGroupService,
            IRemoteSearchService remoteSearchService,
            IColidEntrySubscriptionService colidEntrySubscriptionService,
            IMessageTemplateService messageTemplateService,
            IMapper mapper, ILogger<UserService> logger) : base(repo)
        {
            _consumerGroupService = consumerGroupService;
            _colidEntrySubscriptionService = colidEntrySubscriptionService;
            _messageTemplateService = messageTemplateService;
            _mapper = mapper;
            _logger = logger;
            _remoteSearchService = remoteSearchService;
        }

        public User GetOne(Guid userId)
        {
            return GetOne(u => u.Id.Equals(userId), string.Join(',', _includeProperties));
        }

        public async Task<User> GetOneAsync(Guid userId)
        {
            return await GetOneAsync(u => u.Id.Equals(userId), string.Join(',', _includeProperties));
        }

        public User Create(UserDto userDto)
        {
            var userEntity = CheckAndPrepareUserEntityForCreate(userDto);
            Create(userEntity);
            Save();
            return userEntity;
        }

        public void Delete(Guid userId)
        {
            base.Delete(userId);
            Save();
        }

        public async Task<User> CreateAsync(UserDto userDto)
        {
            var userEntity = CheckAndPrepareUserEntityForCreate(userDto);

            Create(userEntity);
            await SaveAsync();

            return userEntity;
        }

        private User CheckAndPrepareUserEntityForCreate(UserDto userDto)
        {
            Guard.IsNotNull(userDto);
            Guard.IsValidEmail(userDto.EmailAddress);

            if (TryGetOne(out var entity, u => u.Id.Equals(userDto.Id)))
            {
                throw new EntityAlreadyExistsException("Couldn't create a new user, because a user with the id already exists", entity);
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.MessageConfig = new MessageConfig
            {
                SendInterval = Common.Enums.SendInterval.Weekly,
                DeleteInterval = Common.Enums.DeleteInterval.Monthly
            };

            return userEntity;
        }

        public async Task<Guid> GetIdByEmailAddressAsync(string emailAddress)
        {
            Guard.IsValidEmail(emailAddress);

            var user = await GetOneAsync(u => u.EmailAddress.Equals(emailAddress));

            if (user.Id == Guid.Empty)
            {
                throw new EntityNotFoundException($"No user with email {emailAddress} found", emailAddress);
            }

            return user.Id;
        }

        public async Task<User> UpdateEmailAddressAsync(Guid userId, string email)
        {
            Guard.IsValidEmail(email);
            var user = await GetOneAsync(u => u.Id.Equals(userId));
            user.EmailAddress = email;
            Update(user);
            await SaveAsync();

            return user;
        }

        #region [Default Consumer Group]

        public async Task<ConsumerGroup> GetDefaultConsumerGroupAsync(Guid userId)
        {
            Guard.IsNotNull(userId);

            var cg = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.DefaultConsumerGroup));

            if (cg.IsEmpty() || cg.DefaultConsumerGroup.IsEmpty())
            {
                throw new EntityNotFoundException($"No default consumer group found for user with id {userId}", userId.ToString());
            }

            return cg.DefaultConsumerGroup;
        }

        public async Task<User> UpdateDefaultConsumerGroupAsync(Guid userId, ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(userId);
            Guard.IsValidUri(consumerGroupDto.Uri);

            var consumerGroup = _consumerGroupService.GetOne(consumerGroupDto.Uri);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.DefaultConsumerGroup));

            user.DefaultConsumerGroup = consumerGroup;
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> RemoveDefaultConsumerGroupAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var user = await GetOneAsync(u => u.Id.Equals(userId),nameof(User.DefaultConsumerGroup));
            user.DefaultConsumerGroup = null;

            // TODO CK: FIX THIS
            // return _userRepo.UpdateReference(userEntity, u => u.DefaultConsumerGroup);
            Update(user);
            await SaveAsync();

            return user;
        }

        #endregion [Default Consumer Group]

        #region [Last Login fields]

        public async Task<User> UpdateLastLoginDataMarketplaceAsync(Guid userId, DateTime time)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId));
            user.LastLoginDataMarketplace = time;
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> UpdateLastLoginColidAsync(Guid userId, DateTime time)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId));
            user.LastLoginEditor = time;
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> UpdateLastTimeCheckedAsync(Guid userId, DateTime time)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId));
            user.LastTimeChecked = time;
            Update(user);
            await SaveAsync();

            return user;
        }

        #endregion [Last Login fields]

        #region [SearchFilter Editor]

        public async Task<SearchFilterEditor> GetSearchFilterEditorAsync(Guid userId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFilterEditor));

            if (user.SearchFilterEditor.IsEmpty())
            {
                throw new EntityNotFoundException($"No editor search filter found for user with id {userId}", userId.ToString());
            }

            return user.SearchFilterEditor;
        }

        public async Task<User> UpdateSearchFilterEditorAsync(Guid userId, JObject jsonString)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFilterEditor));

            var searchFilterEditor = new SearchFilterEditor { FilterJson = jsonString };
            user.SearchFilterEditor = searchFilterEditor;
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> RemoveSearchFilterEditorAsync(Guid userId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFilterEditor));
            user.SearchFilterEditor = null;

            // TODO CK: FIX THIS
            // return _userRepo.UpdateReference(userEntity, u => u.DefaultConsumerGroup);
            Update(user);
            await SaveAsync();

            return user;
        }

        #endregion [SearchFilter Editor]

        #region [Default SearchFilter Data Marketplace]

        public async Task<SearchFilterDataMarketplace> GetDefaultSearchFilterDataMarketplaceAsync(Guid userId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));

            var defaultSearchFilter = user.SearchFiltersDataMarketplace.FirstOrDefault(e => e.Id.Equals(user.DefaultSearchFilterDataMarketplace));
            if (defaultSearchFilter.IsEmpty())
            {
                throw new EntityNotFoundException($"No default data marketplace search filter found for user {userId}", userId.ToString());
            }
            return defaultSearchFilter;
        }

        public async Task<User> UpdateDefaultSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId)
        {
            Guard.IsNotNull(userId);
            Guard.IsPositiveInteger(searchFilterId);
            // TODO CK: Build searchfilterdmp Repository for all users and fetch Filters from there. Check if Exists, TryGetOne, etc.

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));

            if (user.SearchFiltersDataMarketplace.FirstOrDefault(sf => sf.Id.Equals(searchFilterId)).IsEmpty())
            {
                throw new EntityNotFoundException($"No data marketplace search filter with the id {searchFilterId} for the user {userId} exists");
            }

            user.DefaultSearchFilterDataMarketplace = searchFilterId;
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> RemoveDefaultSearchFilterDataMarketplaceAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));
            user.DefaultSearchFilterDataMarketplace = null;

            // TODO CK: test this
            Update(user);
            await SaveAsync();

            return user;
        }

        #endregion [Default SearchFilter Data Marketplace]

        #region [SearchFilter Data Marketplace]

        public async Task<SearchFilterDataMarketplace> GetSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId)
        {
            Guard.IsNotNull(userId, searchFilterId);

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));
            var searchFilter = user.SearchFiltersDataMarketplace.FirstOrDefault(sf => sf.Id.Equals(searchFilterId));

            if (searchFilter.IsEmpty())
            {
                throw new EntityNotFoundException($"No Data Marketplace search filter found for user with id {userId}");
            }

            return searchFilter;
        }

        public async Task<ICollection<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace) + "," + nameof(User.SearchFiltersDataMarketplace) + ".StoredQuery");
            var searchFilterDataMarketplace = user.SearchFiltersDataMarketplace;

            if (searchFilterDataMarketplace == null || !searchFilterDataMarketplace.Any())
            {
                throw new EntityNotFoundException($"No Data Marketplace search filter found for user with id {userId}");
            }

            return searchFilterDataMarketplace;
        }

        public async Task<User> AddSearchFilterDataMarketplaceAsync(Guid userId, SearchFilterDataMarketplaceDto searchFilterDto)
        {
            Guard.IsNotNull(userId, searchFilterDto);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));
            var searchFilter = _mapper.Map<SearchFilterDataMarketplace>(searchFilterDto);
            searchFilter.User = user;
            user.SearchFiltersDataMarketplace ??= new Collection<SearchFilterDataMarketplace>();
            user.SearchFiltersDataMarketplace.Add(searchFilter);
            // TODO CK: test this
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<User> RemoveSearchFilterDataMarketplaceAsync(Guid userId, int id)
        {
            Guard.IsNotNull(userId, id);

            // direct access to repo here, because this is the only possible function for a service yet.
            var searchFilter = _repo.GetOne<SearchFilterDataMarketplace>(sf => sf.Id.Equals(id),nameof(SearchFilterDataMarketplace.StoredQuery));
            if (searchFilter.IsEmpty())
            {
                throw new EntityNotFoundException($"The given search filter {id} for the user {userId} does not exist!");
            }
            if (!searchFilter.StoredQuery.IsEmpty())
            {
                await RemoveStoredQueryFromSearchFiltersDataMarketplaceAync(userId, id);
            }

            _repo.Delete(searchFilter);
            await _repo.SaveAsync();

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace));

            return user;
        }

        #endregion [SearchFilter Data Marketplace]

        #region [StoredQueries]

        public async Task<ICollection<User>> GetAllSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync()
        {
            var users = await GetAllAsync(null, nameof(User.SearchFiltersDataMarketplace) +","+nameof(User.SearchFiltersDataMarketplace) + ".StoredQuery");
            users.ToList().ForEach(x => x.SearchFiltersDataMarketplace = x.SearchFiltersDataMarketplace.Where(y => y.StoredQuery.IsNotEmpty()).ToList());
            users = users.Where(x => x.SearchFiltersDataMarketplace.IsNotNullAndEmpty());

            if (users == null || !users.Any())
            {
                return new List<User>();
            }

            return users.ToList();
        }

        public async Task<ICollection<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync(Guid userId)
        {
            Guard.IsNotNull(userId);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace)+","+nameof(User.SearchFiltersDataMarketplace)+".StoredQuery");
            var searchFilterDataMarketplace = user.SearchFiltersDataMarketplace.Where(x=> !x.StoredQuery.IsEmpty()).ToList();

            if (searchFilterDataMarketplace == null || !searchFilterDataMarketplace.Any())
            {
                throw new EntityNotFoundException($"No Data Marketplace search filter found for user with id {userId}");
            }

            return searchFilterDataMarketplace;
        }

        public async Task<SearchFilterDataMarketplace> AddStoredQueryToSearchFiltersDataMarketplaceAync(Guid userId, StoredQueryDto storedQueryDto)
        {
            Guard.IsNotNull(userId, storedQueryDto);
            var searchFilter = await RemoveStoredQueryFromSearchFiltersDataMarketplaceAync(userId, storedQueryDto.SearchFilterDataMarketplaceId);
            var storedQuery = _mapper.Map<StoredQuery>(storedQueryDto);
            searchFilter.StoredQuery = storedQuery;
            _repo.Update<SearchFilterDataMarketplace>(searchFilter);
            await SaveAsync(); 

            return searchFilter;
        }

        public async Task<SearchFilterDataMarketplace> RemoveStoredQueryFromSearchFiltersDataMarketplaceAync(Guid userId, int SearchFilterDataMarketplaceId)
        {
            Guard.IsNotNull(userId, SearchFilterDataMarketplaceId);
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.SearchFiltersDataMarketplace) + "," + nameof(User.SearchFiltersDataMarketplace) + ".StoredQuery");
            var searchFilter = user.SearchFiltersDataMarketplace.FirstOrDefault(sf => sf.Id.Equals(SearchFilterDataMarketplaceId));

            if (user.IsEmpty())
            {
                throw new EntityNotFoundException($"No user with id {userId} found");
            }
            if (searchFilter.IsEmpty())
            {
                throw new EntityNotFoundException($"The given SearchFilterDataMarketplace with Id {SearchFilterDataMarketplaceId} for the user {userId} does not exist!");
            }
            if (searchFilter.StoredQuery != null)
            {
                _repo.Delete(searchFilter.StoredQuery);
                Update(user);
                await SaveAsync();
            }

            return searchFilter;
        }

        #endregion [StoredQueries]

        #region [Colid Entry Subscriptions]

        public async Task<User> AddColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(userId, colidEntrySubscriptionDto);

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.ColidEntrySubscriptions));

            var colidEntrySubscription = _mapper.Map<ColidEntrySubscription>(colidEntrySubscriptionDto);
            colidEntrySubscription.User = user;
            user.ColidEntrySubscriptions ??= new Collection<ColidEntrySubscription>();

            // Check if the user is already subscribed to this resource
            var existingSubscription = user.ColidEntrySubscriptions
                .Where(ces => colidEntrySubscription.ColidPidUri.AbsoluteUri == ces.ColidPidUri.AbsoluteUri)
                .FirstOrDefault();
            if (existingSubscription.IsNotEmpty())
            {
                throw new EntityAlreadyExistsException("The user is already subscribed to the COLID entry.", existingSubscription);
            }

            user.ColidEntrySubscriptions.Add(colidEntrySubscription);
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task<ICollection<ColidEntrySubscriptionDto>> GetColidEntrySubscriptionsAsync(Guid userId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.ColidEntrySubscriptions));
            var resultDto = _mapper.Map<ICollection<ColidEntrySubscriptionDto>>(user.ColidEntrySubscriptions);

            return resultDto;
        }

        public async Task<User> RemoveColidEntrySubscriptionAsync(Guid userId, ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(userId, colidEntrySubscriptionDto);

            var colidEntrySubscription = _colidEntrySubscriptionService.GetOne(userId, colidEntrySubscriptionDto);
            if (colidEntrySubscription.IsEmpty())
            {
                throw new EntityNotFoundException($"The given subscription on PidUri {colidEntrySubscriptionDto.ColidPidUri} for the user {userId} does not exist!");
            }
            _colidEntrySubscriptionService.Delete(colidEntrySubscription);
            await _colidEntrySubscriptionService.SaveAsync();

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.ColidEntrySubscriptions));

            return user;
        }

        #endregion [Colid Entry Subscriptions]

        #region [Message Config]

        public async Task<MessageConfig> GetMessageConfigAsync(Guid userId)
        {
            Guard.IsNotNull(userId);

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.MessageConfig));

            if (user.MessageConfig.IsEmpty())
            {
                throw new EntityNotFoundException($"No message config for user {user.Id} exists");
            }
            return user.MessageConfig;
        }

        // check if the given message config is valid, if the values changed and update existing messages for the changed entity
        // pretends, that a message config per user exists
        public async Task<User> UpdateMessageConfigAsync(Guid userId, MessageConfigDto messageConfigDto)
        {
            Guard.IsNotNull(userId, messageConfigDto);
            CheckIfMessageConfigIsValid(messageConfigDto);

            var user = await GetOneAsync(u => u.Id.Equals(userId), $"{nameof(User.MessageConfig)},{nameof(User.Messages)}");

            if (user.MessageConfig.IsEmpty())
            {
                throw new EntityNotFoundException("Couldn't find a message config");
            }

            var sendIntervalUpdated = user.MessageConfig.SendInterval != messageConfigDto.SendInterval;
            var deleteIntervalUpdated = user.MessageConfig.DeleteInterval != messageConfigDto.DeleteInterval;
            if (!sendIntervalUpdated && !deleteIntervalUpdated)
            {
                throw new EntityNotChangedException("Couldn't update the message config, because is hasn't changed");
            }

            user.MessageConfig.SendInterval = messageConfigDto.SendInterval;
            user.MessageConfig.DeleteInterval = messageConfigDto.DeleteInterval;

            if (user.Messages.IsNullOrEmpty())
            {
                Update(user);
                await SaveAsync();

                return user;
            }

            return UpdateMessagesSendOnAndDeleteOnByInterval(user, sendIntervalUpdated, deleteIntervalUpdated);
        }

        /// <summary>
        ///  Based on the given arguments, all messages will be fetched and updated, if SendOn and/or DeleteOn was changed.
        /// </summary>
        private User UpdateMessagesSendOnAndDeleteOnByInterval(User user, bool sendIntervalUpdated, bool deleteIntervalUpdated)
        {
            foreach (var message in user.Messages)
            {
                if (sendIntervalUpdated && message.ReadOn == null)
                {
                    message.SendOn = DateTime.Now.CalculateByInterval(user.MessageConfig.SendInterval);
                }

                if (deleteIntervalUpdated)
                {
                    message.DeleteOn = DateTime.Now.CalculateByInterval(user.MessageConfig.DeleteInterval);
                }
            }

            Update(user);
            Save();

            return user;
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
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));
            // important! ReadDate, SendDate and DeleteDate needs to be set in the messageDto!

            var message = _mapper.Map<Message>(messageDto);
            message.User = user;
            user.Messages ??= new Collection<Message>();
            user.Messages.Add(message);

            Update(user);
            await SaveAsync();

            return user;
        }

        public User AddMessage(Guid userId, MessageDto messageDto)
        {
            var user = GetOne(u => u.Id.Equals(userId), nameof(User.Messages));
            var message = _mapper.Map<Message>(messageDto);
            user.Messages ??= new List<Message>();
            user.Messages.Add(message);
            Update(user);
            Save();

            return user;
        }

        public async Task<User> AddMessagesAsync(Guid userId, ICollection<MessageDto> messages)
        {
            Guard.IsNotNull(userId);
            Guard.IsNotNullOrEmpty(messages);

            var user = GetOne(u => u.Id.Equals(userId), nameof(User.Messages));
            user.Messages ??= new Collection<Message>();
            foreach (var message in messages)
            {
                var messageEntity = _mapper.Map<Message>(message);
                messageEntity.User = user;
                user.Messages.Add(messageEntity);
            }

            // TODO ck: TEST THIS !!
            Update(user);
            await SaveAsync();

            return user;
        }

        public async Task DeleteMessageAsync(Guid userId, int messageId)
        {
            Guard.IsNotNull(userId, messageId);
            Guard.IsPositiveInteger(messageId);

            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));

            var message = user.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message.IsEmpty())
            {
                throw new EntityNotFoundException($"No message with the id {messageId} for user {userId} exists");
            }

            user.Messages.Remove(message);
            Update(user);
            await SaveAsync();
        }

        public async Task<ICollection<MessageDto>> GetMessagesAsync(Guid userId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));

            if (user.Messages.IsNullOrEmpty())
            {
                return new Collection<MessageDto>();
            }

            return _mapper.Map<Collection<MessageDto>>(user.Messages);
        }

        public async Task<ICollection<MessageDto>> MarkMessagesAsReadAsync(Guid userId, ICollection<int> messageIds)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));

            foreach (var messageId in messageIds)
            {
                try
                {
                    var userMessage = GetMessageByUserAndId(user, messageId);

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

            Update(user);
            await SaveAsync();

            var messages = user.Messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();

            return messages;
        }

        public async Task<MessageDto> MarkMessageAsReadAsync(Guid userId, int messageId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));
            var userMessage = GetMessageByUserAndId(user, messageId);

            if (userMessage.ReadOn != null)
            {
                return _mapper.Map<MessageDto>(userMessage);
            }

            userMessage.ReadOn = DateTime.Now;
            userMessage.SendOn = null;
            Update(user);
            await SaveAsync();

            return _mapper.Map<MessageDto>(userMessage);
        }

        public async Task<MessageDto> MarkMessageAsSentAsync(Guid userId, int messageId)
        {
            var user = await GetOneAsync(u => u.Id.Equals(userId), nameof(User.Messages));
            var userMessage = GetMessageByUserAndId(user, messageId);

            if (userMessage.SendOn != null)
            {
                userMessage.SendOn = null;
                userMessage.ReadOn = DateTime.Now.LastDayOfWeek();

                Update(user);
                await SaveAsync();
            }

            return _mapper.Map<MessageDto>(userMessage);
        }

        private Message GetMessageByUserAndId(User user, int messageId)
        {
            Guard.IsPositiveInteger(messageId);
            var userMessage = user.Messages?.FirstOrDefault(msg => msg.Id.Equals(messageId));
            if (userMessage.IsEmpty())
            {
                throw new EntityNotFoundException($"No message with the id {messageId} for user {user.Id} exists");
            }

            return userMessage;
        }

        #endregion [Messages]

        #region storedQuery notification

        public async Task ProcessStoredQueries()
        {
            var userSearchfilter = await this.GetAllSearchFiltersDataMarketplaceOnlyWithStoredQueriesAsync();

            for(int i=0; i<userSearchfilter.Count; i++)
            {
                var user = userSearchfilter.ElementAt(i);
                for(int j=0; j<user.SearchFiltersDataMarketplace.Count; j++)
                {
                    var searchfilter = user.SearchFiltersDataMarketplace.ElementAt(j);
                    if (!StoredQueryNeedsToBeEvaluated(searchfilter.StoredQuery))
                    {
                        continue;
                    }
                    var searchResult = await GetElasticSearchResult(searchfilter);
                    string computedHash = this.GetHashOfSearchResults(searchResult);
 
                    bool initalSearch = searchfilter.StoredQuery.SearchResultHash == null;
                    if (initalSearch || computedHash != searchfilter.StoredQuery.SearchResultHash)
                    {
                        List<string> newPids = GetUpdatedResources(searchResult.Hits.Hits, searchfilter.StoredQuery);
                        try
                        {
                            searchfilter.StoredQuery.SearchResultHash = computedHash;
                            searchfilter.StoredQuery.LatestExecutionDate = DateTime.Now;
                            searchfilter.StoredQuery.NumberSearchResults = (int) searchResult.Hits.Total;
                            Update(user);
                            Save();
                        }
                        catch
                        {
                            throw new EntityNotChangedException("Couldn't update the stored query of the user");
                        }
                        if (newPids.Count > 0)
                        {
                            await NotifyUserAboutUpdates(searchfilter, newPids);
                        }
                    } 
                }
            }
        }

        public string GetHashOfSearchResults(SearchResultDTO searchResult)
        {
            Guard.IsNotNull(searchResult);
            int i = 0;
            var batchList = searchResult.Hits.Hits.GroupBy(x => (i++ / 30)).ToList();
            string joinedHitValues = "";

            using SHA256 sha256 = SHA256.Create();
            foreach (var batch in batchList)
            {
                string tempStringToHash = string.Join(";", batch.Select(x => "{" + string.Join(",", x.Source.Select(kv => kv.Key + "=" + kv.Value.ToString())) + "}"));
                joinedHitValues = joinedHitValues + HashGenerator.GetHash(sha256, tempStringToHash) + "\n";
            }

            var computedHash = HashGenerator.GetHash(sha256, joinedHitValues);
            return computedHash;
        }

        public async Task NotifyUserAboutUpdates(SearchFilterDataMarketplace sf, List<string> newPids)
        {
            Guard.IsNotNull(sf, newPids);
            string subject = " ";
            string body = " ";
            var messageConfig = await this.GetMessageConfigAsync(sf.User.Id);
            var sendOn = DateTime.Now.CalculateByInterval(messageConfig.SendInterval);
            var deleteOn = DateTime.Now.CalculateByInterval(messageConfig.DeleteInterval);
            if (sendOn != null)
            {
                deleteOn = sendOn.Value.CalculateByInterval(messageConfig.DeleteInterval);
            }

            var messageTemplate = _messageTemplateService.GetOne(MessageType.StoredQueryResult);
            subject = messageTemplate.Subject.Replace("%SEARCH_NAME%", $"\"{sf.Name}\"");
            body = messageTemplate.Body.Replace("%UPDATED_RESOURCES%", "<br>"+ string.Join("<br>", newPids.Select(x => "<a href=" + x + " </a>" + x)));

            var messageDto = new MessageDto { Subject = subject, Body = body, SendOn = sendOn, DeleteOn = deleteOn };
            AddMessage(sf.User.Id, messageDto);
        }

        public async Task<SearchResultDTO> GetElasticSearchResult(SearchFilterDataMarketplace searchfilter)
        {
            Guard.IsNotNull(searchfilter);
            var aggregationFilters = searchfilter.FilterJson != null ? searchfilter.FilterJson.GetValue("aggregations").ToObject<Dictionary<string, List<string>>>() : new Dictionary<string, List<string>>();
            var rangeFilters = searchfilter.FilterJson != null ? searchfilter.FilterJson.GetValue("ranges").ToObject<Dictionary<string, Dictionary<string, string>>>() : new Dictionary<string, Dictionary<string, string>>();
            var searchRequest = new SearchRequestDto()
            {
                From = 0,
                Size = 10000,
                SearchTerm = searchfilter.SearchTerm == null ? "*" : searchfilter.SearchTerm,
                     AggregationFilters = aggregationFilters,            
                     RangeFilters = rangeFilters,       
                     EnableHighlighting = false, 
                     ApiCallTime = DateTime.Now.ToString()
            };
            var searchResult =  await _remoteSearchService.Search(searchRequest);

            return searchResult;
        }

        public List<string> GetUpdatedResources(List<SearchHit> hits, StoredQuery storedQuery)
        {
            Guard.IsNotNull(hits, storedQuery);
            var storedQueryLatestExecutionDate = storedQuery.LatestExecutionDate != null ? storedQuery.LatestExecutionDate : storedQuery.CreatedAt;
            var newPids = new List<string>();
            foreach(SearchHit hit in hits)
            { 
                var lastChangeString = JObject.Parse(hit.Source.GetValueOrDefault("https://pid.bayer.com/kos/19050/lastChangeDateTime").ToString()).ToObject<DocumentMapDirection>().outbound.FirstOrDefault().value;
                var dateCreatedString = JObject.Parse(hit.Source.GetValueOrDefault("https://pid.bayer.com/kos/19050/dateCreated").ToString()).ToObject<DocumentMapDirection>().outbound.FirstOrDefault().value;
                var lastChangeDate = DateTime.ParseExact(lastChangeString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var dateCreatedDate = DateTime.ParseExact(dateCreatedString, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                if(dateCreatedDate > storedQueryLatestExecutionDate || lastChangeDate > storedQueryLatestExecutionDate)
                {
                    var pidUri = JObject.Parse(hit.Source.GetValueOrDefault("http://pid.bayer.com/kos/19014/hasPID").ToString()).ToObject<DocumentMapDirection>().outbound.FirstOrDefault().value;
                    newPids.Add(pidUri);
                }
            }

            return newPids;
        } 

        public bool StoredQueryNeedsToBeEvaluated(StoredQuery storedQuery)
        {
            Guard.IsNotNull(storedQuery);
            var executionInterval = storedQuery.ExecutionInterval;
            var storedQueryLatestExecutionDate = storedQuery.LatestExecutionDate != null ? storedQuery.LatestExecutionDate : storedQuery.CreatedAt;

            switch (executionInterval)
            {
                case ExecutionInterval.Daily:
                    return storedQueryLatestExecutionDate.Value.AddDays(1) <= DateTime.Now;
                case ExecutionInterval.Weekly:
                    return storedQueryLatestExecutionDate.Value.AddDays(7) <= DateTime.Now;
                case ExecutionInterval.Monthly:
                    return storedQueryLatestExecutionDate.Value.AddMonths(1) <= DateTime.Now;
            }
            return false;
        }
        #endregion
    }
}
