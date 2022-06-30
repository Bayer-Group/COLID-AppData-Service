using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Tests.Unit;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace COLID.AppDataService.Tests.Integration
{
    public class TestDataContextSeeder
    {
        public static IEnumerable<User> UserList = TestData.GetPreconfiguredUsers();
        public static IEnumerable<ConsumerGroup> ConsumerGroupList = TestData.GetPreconfiguredConsumerGroups();
        public static IEnumerable<SearchFilterEditor> SearchFilterEditorList = TestData.GetPreconfiguredSearchFilterEditor();
        public static IEnumerable<SearchFilterDataMarketplace> SearchFilterDataMarketplaceList = TestData.GetPreconfiguredSearchFiltersDataMarketplace();
        public static IEnumerable<StoredQuery> StoredQueryList = TestData.GetPreconfiguredStoredQueries();
        public static IEnumerable<ColidEntrySubscription> ColidEntrySubscriptionList = TestData.GetPreconfiguredColidEntrySubscriptions();
        public static IEnumerable<MessageTemplate> MessageTemplateList = TestData.GetPreconfiguredMessageTemplates();
        public static IEnumerable<WelcomeMessage> WelcomeMessageList = TestData.GetPreconfiguredWelcomeMessages();

        private readonly DbContextOptions<AppDataContext> _dbOptions;

        public TestDataContextSeeder(DbContextOptions<AppDataContext> dbOptions)
        {
            Guard.IsNotNull(dbOptions);
            _dbOptions = dbOptions;
        }

        public void SeedAll()
        {
            SeedUsers();
            SeedConsumerGroups();
            SeedSearchFiltersEditor();
            SeedSearchFiltersDataMarketplace();
            SeedStoredQueries();
            SeedColidEntrySubscriptions();
            SeedMessageTemplates();
            SeedWelcomeMessages();

            SeedOthers();
        }

        public void SeedUsers()
        {
            Seed(UserList);
        }

        public void AssignMessageConfigToAllUsers()
        {
            foreach (var user in UserList)
            {
                AssignMessageConfigToUsers(user,
                    new MessageConfig {SendInterval = SendInterval.Weekly, DeleteInterval = DeleteInterval.Monthly});
            }
        }

        public void ClearUsers()
        {
            Remove<User>();
        }

        public void ResetUsers()
        {
            Remove<User>();
            Seed(UserList);
        }

        public void SeedConsumerGroups()
        {
            Seed(ConsumerGroupList);
        }

        public void ResetConsumerGroups()
        {
            Remove<ConsumerGroup>();
            Seed(ConsumerGroupList);
        }

        public void SeedSearchFiltersEditor()
        {
            Seed(SearchFilterEditorList);
        }

        public void ResetSearchFiltersEditor()
        {
            Remove<SearchFilterEditor>();
            Seed(SearchFilterEditorList);
        }

        public void SeedSearchFiltersDataMarketplace()
        {
            Seed(SearchFilterDataMarketplaceList);
        }

        public void ResetSearchFiltersDataMarketplace()
        {
            Remove<SearchFilterDataMarketplace>();
            Seed(SearchFilterDataMarketplaceList);
        }

        public void SeedStoredQueries()
        {
            Seed(StoredQueryList);
        }

        public void ResetStoredQueries()
        {
            Remove<StoredQuery>();
            Seed(StoredQueryList);
        }

        public void SeedColidEntrySubscriptions()
        {
            Seed(ColidEntrySubscriptionList);
        }

        public void ClearMessages()
        {
            Remove<Message>();
        }

        public void ResetColidEntrySubscriptions()
        {
            Remove<ColidEntrySubscription>();
            Seed(ColidEntrySubscriptionList);
        }

        public void ClearColidEntrySubscriptions()
        {
            Remove<ColidEntrySubscription>();
        }

        public void SeedMessageTemplates()
        {
            Seed(MessageTemplateList);
        }

        public void ClearMessageTemplates()
        {
            Remove<MessageTemplate>();
        }

        public void ResetMessageTemplates()
        {
            Remove<MessageTemplate>();
            Seed(MessageTemplateList);
        }
        public void SeedWelcomeMessages()
        {
            Seed(WelcomeMessageList);
        }

        public void ResetWelcomeMessages()
        {
            Remove<WelcomeMessage>();
            Seed(WelcomeMessageList);
        }

        // =========================================

        public User AppendSearchFilterEditorToUser(SearchFilterEditor sf, User user)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.SearchFilterEditor = sf;
            return UpdateAndSaveUser(context, dbUser);
        }

        public User AppendSearchFilterDataMarketplaceToUser(SearchFilterDataMarketplace sf, User user)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            if (dbUser.SearchFiltersDataMarketplace == null)
            {
                dbUser.SearchFiltersDataMarketplace = new Collection<SearchFilterDataMarketplace>();
            }
            dbUser.SearchFiltersDataMarketplace.Add(sf);
            return UpdateAndSaveUser(context, dbUser);
        }
        public User AppendStoredQueryToSearchFilterDataMarketplace(User user,SearchFilterDataMarketplace sf, StoredQuery sq)
        {
            using var context = new AppDataContext(_dbOptions);
            var searchfilter = user.SearchFiltersDataMarketplace.Where(x=>x.Id==sf.Id).FirstOrDefault();
            searchfilter.StoredQuery = sq;
            return UpdateAndSaveUser(context, user);
        }
        public User SetDefaultSearchFilterDataMarketplaceToUser(int sfId, User user)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.DefaultSearchFilterDataMarketplace = sfId;
            return UpdateAndSaveUser(context, dbUser);
        }

        public ColidEntrySubscription AppendColidEntrySubscriptionToUser(ColidEntrySubscription ce, User user)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSubscription = context.ColidEntrySubscriptions.Find(ce.Id);
            if (dbSubscription == null)
            {
                throw new EntityNotFoundException("Doesn't exist.");
            }
            dbSubscription.User = user;
            var result = context.Update(dbSubscription);
            context.SaveChanges();

            return result.Entity;
        }

        public User AppendMessageConfigToUser(MessageConfig msgCfg, User user)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.MessageConfig = msgCfg;
            return UpdateAndSaveUser(context, dbUser);
        }

        // =========================================

        /// Adds a specific entity to the context
        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSet = context.Set<TEntity>();
            var dbEntity = dbSet.Add(entity);
            context.SaveChanges();
            return dbEntity.Entity;
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSet = context.Set<TEntity>();
            var dbEntity = dbSet.Update(entity);
            context.SaveChanges();
            return dbEntity.Entity;
        }

        public IList<TEntity> GetAll<TEntity>(string includeProperties = null) where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            IQueryable<TEntity> query = context.Set<TEntity>();

            includeProperties ??= string.Empty;

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        // =========================================

        private void Seed<TEntity>(TEntity entity) where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSet = context.Set<TEntity>();
            if (!dbSet.Any())
            {
                dbSet.Add(entity);
                context.SaveChanges();
            }
        }

        private void Seed<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSet = context.Set<TEntity>();
            if (!dbSet.Any())
            {
                dbSet.AddRange(entityList);
                context.SaveChanges();
            }
        }

        private void Remove<TEntity>() where TEntity : class
        {
            using var context = new AppDataContext(_dbOptions);
            var dbSet = context.Set<TEntity>();
            if (dbSet.Any())
            {
                var entities = dbSet.ToList();
                dbSet.RemoveRange(entities);
                context.SaveChanges();
            }
        }

        // =========================================

        public void SeedOthers()
        {
            using var context = new AppDataContext(_dbOptions);

            /* FIXME CK.. not now
            // Fetch template for colid entry subscription -- fetch user configs and messages with this type -- assign template to user config and messages
            var messageTemplateForColidEntrySubscription = context.MessageTemplates.Where(m => m.Type.Equals(MessageType.ColidEntrySubscription)).FirstOrDefault();
            context.UserMessageConfigs.Where(m => m.MessagesType.Equals(MessageType.ColidEntrySubscription))
                .ToList().ForEach(cfg => AssignMessageTemplateToUserMessageConfig(messageTemplateForColidEntrySubscription, cfg));
            context.Messages.Where(m => m.Type.Equals(MessageType.ColidEntrySubscription))
                .ToList().ForEach(msg => AssignMessageTemplateToMessage(messageTemplateForColidEntrySubscription, msg));

            // Fetch template for stored query result -- fetch user configs and messages with this type -- assign template to user config and messages
            var messageTemplateForStoredQueryResult = context.MessageTemplates.Where(m => m.Type.Equals(MessageType.StoredQueryResult)).FirstOrDefault();
            context.UserMessageConfigs.Where(m => m.MessagesType.Equals(MessageType.StoredQueryResult))
                .ToList().ForEach(cfg => AssignMessageTemplateToUserMessageConfig(messageTemplateForStoredQueryResult, cfg));
            context.Messages.Where(m => m.Type.Equals(MessageType.StoredQueryResult))
                .ToList().ForEach(msg => AssignMessageTemplateToMessage(messageTemplateForStoredQueryResult, msg));
            */

            // Apply
            var userList = context.Users.ToList();
            var consumerGroupList = context.ConsumerGroups.ToList();
            var searchFilterEditorList = context.SearchFiltersEditor.ToList();
            var searchFilterDataMarketplaceList = context.SearchFilterDataMarketplace.ToList();
            var storedQueriesList = context.StoredQueries.ToList();
            var colidEntrySubscriptionList = context.ColidEntrySubscriptions.ToList();
            var messageTemplateList = context.MessageTemplates.ToList();
            var messageList = context.Messages.ToList();
            var userMessageConfigList = context.MessageConfigs.ToList();


            // 4 consumer groups - 5 users
            AssignDefaultConsumerGroupToUser(userList.ElementAt(0), consumerGroupList.ElementAt(0));
            AssignDefaultConsumerGroupToUser(userList.ElementAt(1), consumerGroupList.ElementAt(1));
            AssignDefaultConsumerGroupToUser(userList.ElementAt(2), consumerGroupList.ElementAt(2));
            AssignDefaultConsumerGroupToUser(userList.ElementAt(3), consumerGroupList.ElementAt(3));

            // 3 search filter editor - 5 users
            AssignSearchFilterEditorToUser(userList.ElementAt(0), searchFilterEditorList.ElementAt(0));
            AssignSearchFilterEditorToUser(userList.ElementAt(1), searchFilterEditorList.ElementAt(1));
            AssignSearchFilterEditorToUser(userList.ElementAt(2), searchFilterEditorList.ElementAt(2));

            // 3 search filter dmp - 5 users
            AssignSearchFilterDatamarketPlaceToUser(userList.ElementAt(2), searchFilterDataMarketplaceList.GetRange(0, 2));
            AssignSearchFilterDatamarketPlaceToUser(userList.ElementAt(4), searchFilterDataMarketplaceList.GetRange(2, 1));

            // 3 stored queries - 5 users
            AssignStoredQueryToSearchDatamarketPlace(searchFilterDataMarketplaceList.ElementAt(0), storedQueriesList.ElementAt(0));
            AssignStoredQueryToSearchDatamarketPlace(searchFilterDataMarketplaceList.ElementAt(1), storedQueriesList.ElementAt(1));
            AssignStoredQueryToSearchDatamarketPlace(searchFilterDataMarketplaceList.ElementAt(2), storedQueriesList.ElementAt(2));

            // 4 colid entry subscriptions - 5 users
            AssignColidEntrySubscriptionsToUser(userList.ElementAt(0), colidEntrySubscriptionList.GetRange(0, 1));
            AssignColidEntrySubscriptionsToUser(userList.ElementAt(1), colidEntrySubscriptionList.GetRange(1, 1));
            AssignColidEntrySubscriptionsToUser(userList.ElementAt(2), colidEntrySubscriptionList.GetRange(2, 2));

            /*
            AssignUserMessageConfigsToUser(userList.ElementAt(3), userMessageConfigList.Where(m => m.MessagesType.Equals(MessageType.StoredQueryResult)).ToList());
            AssignUserMessageConfigsToUser(userList.ElementAt(4), userMessageConfigList.Where(m => m.MessagesType.Equals(MessageType.ColidEntrySubscription)).ToList());

            AssignMessagesToUser(userList.ElementAt(3), messageList.Where(m => m.Type.Equals(MessageType.StoredQueryResult)).ToList());
            AssignMessagesToUser(userList.ElementAt(4), messageList.Where(m => m.Type.Equals(MessageType.StoredQueryResult)).ToList());
            */
        }

        #region User modification
        public void AssignMessageConfigToUsers(User user, MessageConfig msgCfg)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.MessageConfig = msgCfg;
            UpdateAndSaveUser(context, dbUser);
        }

        public void AssignDefaultConsumerGroupToUser(User user, ConsumerGroup cg)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.DefaultConsumerGroup = cg;
            UpdateAndSaveUser(context, dbUser);
        }

        public void AssignSearchFilterEditorToUser(User user, SearchFilterEditor sfc)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.SearchFilterEditor = sfc;
            UpdateAndSaveUser(context, dbUser);
        }

        public void AssignSearchFilterDatamarketPlaceToUser(User user, ICollection<SearchFilterDataMarketplace> sfdmp)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.SearchFiltersDataMarketplace = sfdmp;
            UpdateAndSaveUser(context, dbUser);
        }

        public void AssignStoredQueryToSearchDatamarketPlace(SearchFilterDataMarketplace sf, StoredQuery sq)
        {
            using var context = new AppDataContext(_dbOptions);
            var storedqiery = context.StoredQueries.Find(sq.Id);
            var searchfilter = context.SearchFilterDataMarketplace.Find(sf.Id);
            searchfilter.StoredQuery = sq;
            this.Update<SearchFilterDataMarketplace>(searchfilter); 
        }
        public void AssignColidEntrySubscriptionsToUser(User user, ICollection<ColidEntrySubscription> crss)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.ColidEntrySubscriptions = crss;
            UpdateAndSaveUser(context, dbUser);
        }

        public void AssignMessagesToUser(User user, ICollection<Message> msgs)
        {
            using var context = new AppDataContext(_dbOptions);
            var dbUser = context.Users.Find(user.Id);
            dbUser.Messages = msgs;
            UpdateAndSaveUser(context, dbUser);
        }

        private User UpdateAndSaveUser(AppDataContext context, User user)
        {
            var result = context.Update(user);
            context.SaveChanges();
            return result.Entity;
        }

        #endregion User modification
    }
}
