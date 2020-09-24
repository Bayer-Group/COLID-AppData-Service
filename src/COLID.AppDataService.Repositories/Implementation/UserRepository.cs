using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(AppDataContext context) : base(context)
        {
            AddInclude(user => user.DefaultConsumerGroup);
            AddInclude(user => user.SearchFilterEditor);
            AddInclude(user => user.SearchFiltersDataMarketplace);
            AddInclude(user => user.StoredQueries);
            AddInclude(user => user.ColidEntrySubscriptions);
            AddInclude(user => user.MessageConfig);
            AddInclude(user => user.Messages);
        }

        public async Task<Guid> GetIdByEmailAddressAsync(string emailAddress)
        {
            var userId = await FindByCondition(u => u.EmailAddress.Equals(emailAddress))
                .Select(u => u.Id)
                .SingleOrDefaultAsync();

            if (userId == Guid.Empty)
            {
                throw new EntityNotFoundException($"No user with email {emailAddress} found", emailAddress);
            }

            return userId;
        }

        public async Task<ConsumerGroup> GetDefaultConsumerGroupAsync(Guid userId)
        {
            var cg = await FindByCondition(u => u.Id.Equals(userId))
                .Select(u => u.DefaultConsumerGroup)
                .SingleOrDefaultAsync();

            if (cg.IsEmpty())
            {
                throw new EntityNotFoundException($"No default consumer group found for user with id {userId}", userId.ToString());
            }

            return cg;
        }

        public async Task<SearchFilterEditor> GetSearchFilterEditorAsync(Guid userId)
        {
            var sfe = await FindByCondition(u => u.Id.Equals(userId))
                .Select(u => u.SearchFilterEditor)
                .SingleOrDefaultAsync();

            if (sfe.IsEmpty())
            {
                throw new EntityNotFoundException($"No Editor search filter found for user with id {userId}", userId.ToString());
            }

            return sfe;
        }

        public async Task<IList<SearchFilterDataMarketplace>> GetSearchFiltersDataMarketplaceAsync(Guid userId)
        {
            var sfD = await FindByCondition(u => u.Id.Equals(userId))
                .SelectMany(u => u.SearchFiltersDataMarketplace)
                .ToListAsync();

            if (sfD == null || !sfD.Any())
            {
                throw new EntityNotFoundException($"No Data Marketplace search filter found for user with id {userId}");
            }

            return sfD;
        }

        public async Task<SearchFilterDataMarketplace> GetSearchFilterDataMarketplaceAsync(Guid userId, int searchFilterId)
        {
            var sfD = await FindByCondition(u => u.Id.Equals(userId))
                .SelectMany(u => u.SearchFiltersDataMarketplace)
                .Where(x => x.Id.Equals(searchFilterId))
                .SingleOrDefaultAsync();

            if (sfD.IsEmpty())
            {
                throw new EntityNotFoundException($"No Data Marketplace search filter found with the id {searchFilterId} for user with id {userId}");
            }

            return sfD;
        }
    }
}
