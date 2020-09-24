using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class MessageTemplateBuilder : EntityBuilderBase<MessageTemplateBuilder, MessageTemplate, MessageTemplateDto, int>
    {
        protected override MessageTemplateBuilder SpecificBuilder => this;

        public MessageTemplateBuilder()
        {
            _entity = new MessageTemplate();
        }

        public MessageTemplateBuilder WithType(MessageType value)
        {
            _entity.Type = value;
            return this;
        }

        public MessageTemplateBuilder WithSubject(string value)
        {
            _entity.Subject = value;
            return this;
        }

        public MessageTemplateBuilder WithBody(string value)
        {
            _entity.Body = value;
            return this;
        }
    }
}
