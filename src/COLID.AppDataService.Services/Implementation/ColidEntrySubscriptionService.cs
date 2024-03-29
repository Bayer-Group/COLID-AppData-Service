﻿using System;
using System.Collections.Generic;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Services.Interfaces;
using COLID.Exception.Models.Business;
using Common.DataModels.TransferObjects;
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

        public Dictionary<string, int> GetAllSubscriptionsCount()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            var allUserSubscriptions = GetAll(null, "User")
                .AsEnumerable()
                .GroupBy(ce => ce.User)
                .ToList();

            foreach (var subscription in allUserSubscriptions)
            {
                result.Add(subscription.Key.Id.ToString(), subscription.Key.ColidEntrySubscriptions.Count());
            }

            return result;
        }

        public IEnumerable<ColidEntryMostSubscriptionsDto> GetMostSubscribedResources(int take)
        {
            var result = GetAll()
                .GroupBy(sub => sub.ColidPidUri)
                .Select(group => new ColidEntryMostSubscriptionsDto { ResourcePidUri = group.Key, SubscriptionsCount = group.Count() })
                .OrderByDescending(g => g.SubscriptionsCount)
                .Take(take);
            return result;
        }

        public void Delete(ColidEntrySubscriptionDto dto)
        {
            Guard.IsNotNull(dto);
            Guard.IsValidUri(dto.ColidPidUri);

            var subscriptions = GetAll()
                .AsEnumerable()
                .Where(ce => ce.ColidPidUri.AbsoluteUri == dto.ColidPidUri.AbsoluteUri)
                .ToList();

            if (subscriptions.Any())
            {
                DeleteRange(subscriptions);
                Save();
            }
        }
    }
}
