using System;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Cache.Services;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class MessageTemplateService : GenericService<MessageTemplate, int>, IMessageTemplateService
    {
        private readonly IMessageTemplateRepository _mtRepo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly ILogger<MessageTemplateService> _logger;
        public MessageTemplateService(IMessageTemplateRepository mtRepo, IMapper mapper, ICacheService cache, ILogger<MessageTemplateService> logger) : base(mtRepo)
        {
            _mtRepo = mtRepo;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public MessageTemplate GetOne(MessageType type)
        {
            return _cache.GetOrAdd(type.ToString(), () => _mtRepo.GetOne(type));
        }

        public MessageTemplate Create(MessageTemplateDto messageTemplateDto)
        {
            var messageTemplate = CheckAndPrepareMessageTemplateEntityForCreate(messageTemplateDto);
            return _cache.GetOrAdd(messageTemplateDto.Type.ToString(), () => _mtRepo.Create(messageTemplate));
        }

        public async Task<MessageTemplate> CreateAsync(MessageTemplateDto messageTemplateDto)
        {
            var messageTemplate = CheckAndPrepareMessageTemplateEntityForCreate(messageTemplateDto);
            return await _cache.GetOrAddAsync(messageTemplateDto.Type.ToString(), () => _mtRepo.CreateAsync(messageTemplate));
        }

        public MessageTemplate Update(MessageTemplateDto messageTemplateDto)
        {
            Guard.IsNotNull(messageTemplateDto);

            if (_mtRepo.TryGetOne(messageTemplateDto, out var entity))
            {
                throw new EntityNotChangedException("Couldn't update the message template, because is hasn't changed");
            }

            var messageType = messageTemplateDto.Type;
            
            entity = GetOne(messageType);
            entity.Subject = messageTemplateDto.Subject;
            entity.Body = messageTemplateDto.Body;

            return _cache.Update(messageType.ToString(), () => _mtRepo.Update(entity));
        }

        private MessageTemplate CheckAndPrepareMessageTemplateEntityForCreate(MessageTemplateDto messageTemplateDto)
        {
            if (_mtRepo.TryGetOne(messageTemplateDto.Type, out var entity))
            {
                throw new EntityAlreadyExistsException("Couldn't create a new message template, because a similar template already exists", entity);
            }

            var messageTemplate = _mapper.Map<MessageTemplate>(messageTemplateDto);
            return messageTemplate;
        }
    }
}
