using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class UserBuilder : EntityBuilderBase<UserBuilder, User, UserDto, Guid>
    {
        protected override UserBuilder SpecificBuilder => this;

        public UserBuilder WithId(string guid)
        {
            base.WithId(Guid.Parse(guid));
            return this;
        }

        public UserBuilder WithEmailAddress(string email)
        {
            _entity.EmailAddress = email;
            return this;
        }

        public UserBuilder WithLastLoginDataMarketplace(DateTime time)
        {
            _entity.LastLoginDataMarketplace = time;
            return this;
        }

        public UserBuilder WithLastLoginColid(DateTime time)
        {
            _entity.LastLoginEditor = time;
            return this;
        }

        public UserBuilder WithLastTimeChecked(DateTime time)
        {
            _entity.LastTimeChecked = time;
            return this;
        }

        public UserBuilder WithDefaultConsumerGroup(ConsumerGroup dcg)
        {
            _entity.DefaultConsumerGroup = dcg;
            return this;
        }

        public UserBuilder WithSearchFilterColid(SearchFilterEditor searchFilter)
        {
            _entity.SearchFilterEditor = searchFilter;
            return this;
        }

        public UserBuilder WithDefaultSearchFilterDataMarketplaceId(int dsf)
        {
            _entity.DefaultSearchFilterDataMarketplace = dsf;
            return this;
        }

        public UserBuilder WithSearchFilterDataMarketplace(ICollection<SearchFilterDataMarketplace> searchFilter)
        {
            _entity.SearchFiltersDataMarketplace = searchFilter;
            return this;
        }

        public UserBuilder WithStoredQueries(ICollection<StoredQuery> qry)
        {
            _entity.StoredQueries = qry;
            return this;
        }

        public UserBuilder WithColidEntrySubscriptions(ICollection<ColidEntrySubscription> sub)
        {
            _entity.ColidEntrySubscriptions = sub;
            return this;
        }

        public UserBuilder WithMessages(ICollection<Message> msgs)
        {
            _entity.Messages = msgs;
            return this;
        }

        public UserBuilder WithMessageConfig(MessageConfig messageConfig)
        {
            _entity.MessageConfig = messageConfig;
            return this;
        }
    }
}
