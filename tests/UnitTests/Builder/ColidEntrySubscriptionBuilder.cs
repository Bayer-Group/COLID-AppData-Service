using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class ColidEntrySubscriptionBuilder : EntityBuilderBase<ColidEntrySubscriptionBuilder, ColidEntrySubscription, ColidEntrySubscriptionDto, int>
    {
        protected override ColidEntrySubscriptionBuilder SpecificBuilder => this;

        public ColidEntrySubscriptionBuilder()
        {
            _entity = new ColidEntrySubscription();
        }

        public ColidEntrySubscriptionBuilder WithColidEntry(Uri colidEntry)
        {
            _entity.ColidPidUri = colidEntry;
            return this;
        }

        public ColidEntrySubscriptionBuilder WithColidEntry(string colidEntry)
        {
            _entity.ColidPidUri = new Uri(colidEntry);
            return this;
        }
    }
}
