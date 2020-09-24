using System;
using System.Collections.Generic;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class ColidEntrySubscriptionService : GenericService<ColidEntrySubscription, int>, IColidEntrySubscriptionService
    {
        private readonly IColidEntrySubscriptionRepository _ceRepo;
        private readonly ILogger<ColidEntrySubscriptionService> _logger;

        public ColidEntrySubscriptionService(IColidEntrySubscriptionRepository ceRepo, ILogger<ColidEntrySubscriptionService> logger) : base(ceRepo)
        {
            _ceRepo = ceRepo;
            _logger = logger;
        }

        public ColidEntrySubscription GetOne(Guid userId, ColidEntrySubscriptionDto dto)
        {
            Guard.IsNotNull(dto);
            return _ceRepo.GetOne(userId, dto.ColidPidUri);
        }

        public bool TryGetAllUsers(Uri colidPidUri, out IList<User> subscribedUsers)
        {
            Guard.IsValidUri(colidPidUri);
            return _ceRepo.TryGetAllUsers(colidPidUri, out subscribedUsers);
        }

        public IList<ColidEntrySubscriptionAmountDto> GetColidPidUrisAndAmountSubscriptions()
        {
            return _ceRepo.CountSubsciptionsPerColidPidUri();
        }

        public void Delete(ColidEntrySubscriptionDto colidEntrySubscriptionDto)
        {
            Guard.IsNotNull(colidEntrySubscriptionDto);
            Guard.IsValidUri(colidEntrySubscriptionDto.ColidPidUri);

            _ceRepo.Delete(colidEntrySubscriptionDto);
        }
    }
}
