using System;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class ConsumerGroupRepository : GenericRepository<ConsumerGroup, int>, IConsumerGroupRepository
    {
        public ConsumerGroupRepository(AppDataContext context) : base(context)
        {
        }

        public ConsumerGroup GetOne(Uri uri)
        {
            Guard.IsValidUri(uri);
            if (!TryGetOne(uri, out var consumerGroup))
            {
                throw new EntityNotFoundException($"Unable to find a consumer group with the Uri {uri}", uri.AbsoluteUri);
            }

            return consumerGroup;
        }

        public bool TryGetOne(Uri uri, out ConsumerGroup consumerGroup)
        {
            Guard.IsValidUri(uri);
            // This is necessary due to client evaluation, see https://docs.microsoft.com/de-de/ef/core/querying/client-eval for more
            consumerGroup = FindAll()
                .AsEnumerable()
                .Where(cg => cg.Uri.AbsoluteUri == uri.AbsoluteUri)
                .FirstOrDefault();

            return consumerGroup.IsNotEmpty();
        }
    }
}
