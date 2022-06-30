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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using COLID.AppDataService.Common.Extensions;
using COLID.Cache.Services;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Services.Implementation
{
    public class ConsumerGroupService : ServiceBase<ConsumerGroup>, IConsumerGroupService
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ConsumerGroupService> _logger;

        public ConsumerGroupService(IGenericRepository repo, IMapper mapper, ILogger<ConsumerGroupService> logger) : base(repo)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public ConsumerGroup GetOne(Uri uri)
        { 
            Guard.IsValidUri(uri);
            var consumerGroups  = GetAll()
                .AsEnumerable()
                .Where(ce => ce.Uri.AbsoluteUri == uri.AbsoluteUri)
                .ToList();
            var consumerGroup = consumerGroups.SingleOrDefault();
            if (consumerGroup.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a consumer group with the Uri {uri}", uri.AbsoluteUri);
            }

            return consumerGroup;
        }

        public bool TryGetOne(ConsumerGroupDto consumerGroupDto, out ConsumerGroup consumerGroup)
        {
            Guard.IsNotNull(consumerGroupDto);
            Guard.IsValidUri(consumerGroupDto.Uri);

            consumerGroup = null;
            try
            {
                consumerGroup = GetOne(consumerGroupDto.Uri);
            }
            catch (EntityNotFoundException)
            {
            }
            return consumerGroup.IsNotEmpty();
        }

        public ConsumerGroup Create(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);

            if (TryGetOne(consumerGroupDto, out var cgEntity))
            {
                throw new EntityAlreadyExistsException($"This consumer group already exists.", cgEntity);
            }

            cgEntity = _mapper.Map<ConsumerGroup>(consumerGroupDto);
            Create(cgEntity);
            Save();

            return cgEntity;
        }

        public void Delete(ConsumerGroupDto consumerGroupDto)
        {
            Guard.IsNotNull(consumerGroupDto);
            var uri = consumerGroupDto.Uri;

            Guard.IsValidUri(uri);
            var consumerGroup = GetOne(uri);

            Delete(consumerGroup);
            Save();
        }
    }
}
