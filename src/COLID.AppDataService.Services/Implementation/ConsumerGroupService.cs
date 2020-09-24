using System;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using Microsoft.Extensions.Logging;

namespace COLID.AppDataService.Services.Implementation
{
    public class ConsumerGroupService : GenericService<ConsumerGroup, int>, IConsumerGroupService
    {
        private readonly IConsumerGroupRepository _consumerGroupRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ConsumerGroupService> _logger;

        public ConsumerGroupService(IConsumerGroupRepository cgRepo, IMapper mapper, ILogger<ConsumerGroupService> logger) : base(cgRepo)
        {
            _consumerGroupRepo = cgRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public ConsumerGroup GetOne(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);
            var uri = consumerGroupDto.Uri;
            return GetOne(uri);
        }

        public ConsumerGroup GetOne(Uri uri)
        {
            Guard.IsValidUri(uri);
            return _consumerGroupRepo.GetOne(uri);
        }

        public bool TryGetOne(ConsumerGroupDto consumerGroupDto, out ConsumerGroup consumerGroup)
        {
            Guard.IsNotNull(consumerGroupDto);
            var uri = consumerGroupDto.Uri;
            Guard.IsValidUri(uri);
            return _consumerGroupRepo.TryGetOne(uri, out consumerGroup);
        }

        public ConsumerGroup Create(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);

            if (TryGetOne(consumerGroupDto, out var cgEntity))
            {
                throw new EntityAlreadyExistsException($"This consumer group already exists.", cgEntity);
            }

            cgEntity = _mapper.Map<ConsumerGroup>(consumerGroupDto);
            return base.Create(cgEntity);
        }

        public async Task<ConsumerGroup> CreateAsync(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);

            ConsumerGroup cgEntity;
            if (TryGetOne(consumerGroupDto, out cgEntity))
            {
                throw new EntityAlreadyExistsException($"This consumer group already exists.", cgEntity);
            }

            cgEntity = _mapper.Map<ConsumerGroup>(consumerGroupDto);
            return await base.CreateAsync(cgEntity);
        }

        public void Delete(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);
            var uri = consumerGroupDto.Uri;

            Guard.IsValidUri(uri);
            var consumerGroup = _consumerGroupRepo.GetOne(uri);

            base.Delete(consumerGroup);
        }
    }
}
