using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class WelcomeMessageBuilder : EntityBuilderBase<WelcomeMessageBuilder, WelcomeMessage, WelcomeMessageDto, int>
    {
        protected override WelcomeMessageBuilder SpecificBuilder => this;

        public WelcomeMessageBuilder()
        {
            _entity = new WelcomeMessage();
        }

        public WelcomeMessageBuilder WithType(ColidType value)
        {
            _entity.Type = value;
            return this;
        }

        public WelcomeMessageBuilder WithContent(string value)
        {
            _entity.Content = value;
            return this;
        }
    }
}
