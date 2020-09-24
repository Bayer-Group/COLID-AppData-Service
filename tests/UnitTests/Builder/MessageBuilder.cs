using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class MessageBuilder : EntityBuilderBase<MessageBuilder, Message, MessageDto, int>
    {
        protected override MessageBuilder SpecificBuilder => this;

        public MessageBuilder()
        {
            _entity = new Message();
        }

        public MessageBuilder WithSubject(string value)
        {
            _entity.Subject = value;
            return this;
        }

        public MessageBuilder WithBody(string value)
        {
            _entity.Body = value;
            return this;
        }

        public MessageBuilder WithReadOn(DateTime value)
        {
            _entity.ReadOn = value;
            return this;
        }

        public MessageBuilder WithSendOn(DateTime value)
        {
            _entity.SendOn = value;
            return this;
        }

        public MessageBuilder WithDeleteOn(DateTime value)
        {
            _entity.DeleteOn = value;
            return this;
        }

        public MessageBuilder WithUser(User value)
        {
            _entity.User = value;
            return this;
        }
    }
}
