using System;
using System.Linq;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Services.Interfaces;
using COLID.Cache.Services;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class MessageTemplateService : ServiceBase<MessageTemplate>, IMessageTemplateService
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly ILogger<MessageTemplateService> _logger;

        public MessageTemplateService(IGenericRepository repo, IMapper mapper, ICacheService cache, ILogger<MessageTemplateService> logger) : base(repo)
        {
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public MessageTemplate GetOne(MessageType type)
        {
            return _cache.GetOrAdd(type.ToString(), () => GetOne(mt => mt.Type.Equals(type)));
        }

        public MessageTemplate Create(MessageTemplateDto messageTemplateDto)
        {
            var messageTemplate = CheckAndPrepareMessageTemplateEntityForCreate(messageTemplateDto);
            Create(messageTemplate);
            Save();

            return _cache.GetOrAdd(messageTemplateDto.Type.ToString(), () => messageTemplate); // TODO CK: test this !
        }

        public MessageTemplate Update(MessageTemplateDto messageTemplateDto)
        {
            Guard.IsNotNull(messageTemplateDto);

            if (TryGetOne(messageTemplateDto, out var entity))
            {
                throw new EntityNotChangedException("Couldn't update the message template, because is hasn't changed");
            }

            var messageType = messageTemplateDto.Type;

            entity = GetOne(messageType);
            entity.Subject = messageTemplateDto.Subject;
            entity.Body = messageTemplateDto.Body;
            Save();
            _cache.Delete(messageType.ToString(), () => { });


            return entity;
        }

        private MessageTemplate CheckAndPrepareMessageTemplateEntityForCreate(MessageTemplateDto messageTemplateDto)
        {
            if (TryGetOne(messageTemplateDto.Type, out var entity))
            {
                throw new EntityAlreadyExistsException("Couldn't create a new message template, because a similar template already exists", entity);
            }

            var messageTemplate = _mapper.Map<MessageTemplate>(messageTemplateDto);
            return messageTemplate;
        }

        private bool TryGetOne(MessageTemplateDto dto, out MessageTemplate entity)
        {
            Guard.IsNotNull(dto);

            return TryGetOne(out entity, 
                mt =>
                    mt.Type.Equals(dto.Type)
                    && mt.Subject.Equals(dto.Subject, StringComparison.Ordinal)
                    && mt.Body.Equals(dto.Body, StringComparison.Ordinal));
        }

        private bool TryGetOne(MessageType messageType, out MessageTemplate entity)
        {
            return TryGetOne(out entity, mt => mt.Type.Equals(messageType));
        }
    }
}
