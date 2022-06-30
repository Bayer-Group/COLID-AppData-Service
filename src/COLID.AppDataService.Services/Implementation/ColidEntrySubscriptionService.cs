using System;
using System.Collections.Generic;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class ColidEntrySubscriptionService : ServiceBase<ColidEntrySubscription>, IColidEntrySubscriptionService
    {
        private readonly ILogger<ColidEntrySubscriptionService> _logger;

        public ColidEntrySubscriptionService(IGenericRepository repo, ILogger<ColidEntrySubscriptionService> logger) : base(repo)
        {
            _logger = logger;
        }

        public ColidEntrySubscription GetOne(Guid userId, ColidEntrySubscriptionDto dto)
        {
            Guard.IsNotNull(dto);
            Guard.IsValidUri(dto.ColidPidUri);
            var colidPidUri = dto.ColidPidUri.AbsoluteUri;
            var colidEntriesPerUri = GetAll(null, nameof(ColidEntrySubscription.User))
                .AsEnumerable()
                .Where(ce => ce.ColidPidUri.AbsoluteUri == colidPidUri)
                .ToList();
            var colidEntry = colidEntriesPerUri.FirstOrDefault(ce => ce.User.Id.Equals(userId));

            if (colidEntry.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a subscription with the Uri {colidPidUri} for user {userId}", colidPidUri);
            }

            return colidEntry;
        }

        public bool TryGetAllUsers(Uri colidPidUri, out IList<User> subscribedUsers)
        {
            Guard.IsValidUri(colidPidUri);

            subscribedUsers = GetAll(null, nameof(ColidEntrySubscription.User))
                .AsEnumerable()
                .Where(ce => ce.ColidPidUri.AbsoluteUri == colidPidUri.AbsoluteUri)
                .Select(u => u.User)
                .ToList();

            return subscribedUsers.Any();
        }

        public IList<ColidEntrySubscriptionAmountDto> GetColidPidUrisAndAmountSubscriptions(ISet<Uri> colidPidUris)
        {
            Guard.IsNotNullOrEmpty(colidPidUris);

            var results = GetAll()
                .AsEnumerable()
                .GroupBy(ce => ce.ColidPidUri)
                .Where(ce => colidPidUris.Select(u => u.AbsoluteUri).Any(pu => pu == ce.Key.AbsoluteUri))
                .Select(g => new ColidEntrySubscriptionAmountDto { ColidPidUri = g.Key, Subscriptions = g.Count() })
                .ToList();

            // Add the PID URIs that have no subscriptions to the result list
            var missingPidUris = colidPidUris.Where(pu => !results.Any(r => r.ColidPidUri.AbsoluteUri == pu.AbsoluteUri));

            if (missingPidUris.Any())
            {
                var pidUrisWithoutSubscriptions = missingPidUris.Select(uri => new ColidEntrySubscriptionAmountDto { ColidPidUri = uri, Subscriptions = 0 });
                results.AddRange(pidUrisWithoutSubscriptions);
            }

            return results;
        }

        public void Delete(ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(colidEntrySubscriptionDto);
            Guard.IsValidUri(colidEntrySubscriptionDto.ColidPidUri);

            var subscriptions = GetAll()
                .AsEnumerable()
                .Where(ce => ce.ColidPidUri.AbsoluteUri == colidEntrySubscriptionDto.ColidPidUri.AbsoluteUri)
                .ToList();

            if (subscriptions.Any())
            {
                DeleteRange(subscriptions);
                Save();
            }
        }
    }
}
