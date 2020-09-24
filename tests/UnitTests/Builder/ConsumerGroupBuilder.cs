using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class ConsumerGroupBuilder : EntityBuilderBase<ConsumerGroupBuilder, ConsumerGroup, ConsumerGroupDto, int>
    {
        protected override ConsumerGroupBuilder SpecificBuilder => this;

        public ConsumerGroupBuilder()
        {
            _entity = new ConsumerGroup();
        }

        public ConsumerGroupBuilder WithUri(Uri uri)
        {
            _entity.Uri = uri;
            return this;
        }
    }
}
