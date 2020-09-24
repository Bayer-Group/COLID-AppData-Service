using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class MessageTemplateRepository : GenericRepository<MessageTemplate, int>, IMessageTemplateRepository
    {
        public MessageTemplateRepository(AppDataContext context) : base(context)
        {
        }

        public MessageTemplate GetOne(MessageType type)
        {
            var entity = FindByCondition(m => m.Type.Equals(type)).FirstOrDefault();
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a message template for {type}", type.ToString());
            }

            return entity;
        }

        public bool TryGetOne(MessageTemplateDto dto, out MessageTemplate entity)
        {
            Guard.IsNotNull(dto);

            entity = FindAll()
                .Where(mt => mt.Type.Equals(dto.Type))
                .Where(mt => mt.Subject.Equals(dto.Subject))
                .Where(mt => mt.Body.Equals(dto.Body))
                .FirstOrDefault();

            return entity.IsNotEmpty();
        }

        public bool TryGetOne(MessageType messageType, out MessageTemplate entity)
        {
            entity = FindAll()
                .Where(mt => mt.Type.Equals(messageType))
                .FirstOrDefault();

            return entity.IsNotEmpty();
        }
    }
}
