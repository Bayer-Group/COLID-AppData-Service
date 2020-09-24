using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class ColidEntrySubscriptionRepository : GenericRepository<ColidEntrySubscription, int>, IColidEntrySubscriptionRepository
    {
        public ColidEntrySubscriptionRepository(AppDataContext context) : base(context)
        {
        }

        public ColidEntrySubscription GetOne(Guid userId, Uri colidPidUri)
        {
            Guard.IsValidUri(colidPidUri);

            var colidEntriesPerUri = FindAll()
                .Include(ce => ce.User)
                .AsEnumerable() // EFCore cant handle the server-side evaluation dependend relations (userId in this case). Therefore is necessary.
                .Where(ce => ce.ColidPidUri.AbsoluteUri == colidPidUri.AbsoluteUri)
                .ToList();

            var colidEntry = colidEntriesPerUri
                .FirstOrDefault(ce => ce.User.Id.Equals(userId));

            if (colidEntry.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a subscription with the Uri {colidPidUri} for user {userId}", colidPidUri.AbsoluteUri);
            }

            return colidEntry;
        }

        public bool TryGetAllUsers(Uri colidPidUri, out IList<User> subscribedUsers)
        {
            Guard.IsValidUri(colidPidUri);

            subscribedUsers = FindAll()
                .Include(c => c.User)
                .AsEnumerable() // This is necessary due to client evaluation, see https://docs.microsoft.com/de-de/ef/core/querying/client-eval for more
                .Where(ce => ce.ColidPidUri.AbsoluteUri == colidPidUri.AbsoluteUri)
                .Select(u => u.User)
                .ToList();

            return subscribedUsers.Any();
        }

        public void Delete(ColidEntrySubscriptionDto dto)
        {
            Guard.IsNotNull(dto);
            Guard.IsValidUri(dto.ColidPidUri);

            var entities = Context.ColidEntrySubscriptions
                .AsEnumerable() // This is necessary due to client evaluation, see https://docs.microsoft.com/de-de/ef/core/querying/client-eval for more
                .Where(ce => ce.ColidPidUri.AbsoluteUri == dto.ColidPidUri.AbsoluteUri)
                .ToList();

            if (entities.Any())
            {
                base.DeleteRange(entities);
            }
        }

        public IList<ColidEntrySubscriptionAmountDto> CountSubsciptionsPerColidPidUri()
        {
            var query = Context.ColidEntrySubscriptions
                   .GroupBy(ce => ce.ColidPidUri)
                   .Select(g => new ColidEntrySubscriptionAmountDto { ColidPidUri = g.Key, Subscribers = g.Count() });

            return query.ToList();
        }
    }
}
