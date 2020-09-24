using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class MessageConfigBuilder : EntityBuilderBase<MessageConfigBuilder, MessageConfig, MessageConfigDto, int>
    {
        protected override MessageConfigBuilder SpecificBuilder => this;

        public MessageConfigBuilder()
        {
            _entity = new MessageConfig();
        }

        public MessageConfigBuilder WithSendInterval(SendInterval value)
        {
            _entity.SendInterval = value;
            return this;
        }

        public MessageConfigBuilder WithDeleteInterval(DeleteInterval value)
        {
            _entity.DeleteInterval = value;
            return this;
        }

        public MessageConfigBuilder WithUser(User value)
        {
            _entity.User = value;
            return this;
        }
    }
}
